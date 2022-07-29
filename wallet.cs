
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace ValidityAPI
{
    public static class wallet
    {
        private static RPC_Credentials creds = new RPC_Credentials(ConfigFileReader.lookup("rpc_user"), ConfigFileReader.lookup("rpc_pass"), ConfigFileReader.lookup("rpc_ip"), int.Parse(ConfigFileReader.lookup("rpc_port")));
        private static BitnetClient BC = new BitnetClient(creds);

       

        private static Timer timer = new Timer(Update, new AutoResetEvent(false), 1000, 30000);

        public static decimal circ_supply = 0;
        public static decimal netstakeweight = 0;

        public static void Update(Object stateInfo)
        {
            try
            {
                circ_supply = decimal.Parse(BC.gettxoutsetinfo()["total_amount"].ToString());


                JObject stakingInfo = BC.GetStakingInfo();
                Console.WriteLine(stakingInfo);
                string step1 = stakingInfo["netstakeweight"].ToString();
                Console.WriteLine(step1);
                netstakeweight = netstakeweight = Math.Round(decimal.Parse(step1) / 100000000,0); ;
                Console.WriteLine(netstakeweight);
                
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            
            }
            

        }
 

    }

    public class RPC_Credentials
    {
        public RPC_Credentials(string user, string password, string ip, int port)
        {
            User = user;
            Password = password;
            Ip = ip;
            Port = port;
        }

        public string User;
        public string Password;
        public string Ip;
        public int Port;
    }
}