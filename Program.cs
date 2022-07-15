
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace ValidityAPI
{
    public class Program
    {
        public static JArray listen_urls = new JArray();
        public static int listen_port = 80;
        private static X509Certificate2 cert;
        private static bool useSSL = bool.Parse(ConfigFileReader.lookup("useSSL"));

        public static ManualResetEvent shutdown_Event = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) == false)
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            }
            //load config file
            ConfigFileReader.load();
           

            

            //Setup networking
            listen_urls = JArray.Parse(ConfigFileReader.lookup("listens"));
            useSSL = bool.Parse(ConfigFileReader.lookup("useSSL"));

            //setup SSL
            IConfigurationRoot config;
            if (useSSL)
            {
                config = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddEnvironmentVariables()
                             .AddJsonFile("certificate.json", optional: true, reloadOnChange: true)
                             .AddJsonFile($"certificate.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                             .Build();

                var certificateSettings = config.GetSection("certificateSettings");
                string certificateFileName = certificateSettings.GetValue<string>("filename");
                string certificatePassword = certificateSettings.GetValue<string>("password");

                cert = new X509Certificate2(certificateFileName, certificatePassword);
            }
            else
            {
                config = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddEnvironmentVariables()
                                            .Build();
            }
            upbit.Update(new object());
            wallet.Update(new object());
            stakingRewards.Update(new object());

            CreateWebHostBuilder(args, config).Build().RunAsync();

            shutdown_Event.Reset();
            shutdown_Event.WaitOne();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, IConfiguration config) =>
            WebHost.CreateDefaultBuilder(args)
#if true
            .UseKestrel(
                options =>
                {
                    options.AddServerHeader = false;
                    // if SSL is to be used, make sure we listen on SSL port 443 and use our SSL cert.
                    foreach (JObject listen in listen_urls)
                    {
                        if ((bool)listen["SSL"] && useSSL)
                            options.Listen(IPAddress.Parse(listen["listen_url"].ToString()), (int)listen["listen_port"], listenOptions => { listenOptions.UseHttps(cert); });
                        else
                            options.Listen(IPAddress.Parse(listen["listen_url"].ToString()), (int)listen["listen_port"]);
                    }
                }
            )
#endif
             .UseConfiguration(config)
            .UseSetting("https_port", "443")
                .UseStartup<Startup>();
    }
}