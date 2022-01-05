using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace  ValidityAPI


{
    public static class ConfigFileReader
    {

        static bool loaded = false;

        private static JObject options = new JObject();

        public static void load()
        {
            string path = Directory.GetCurrentDirectory().ToString() + "/config.conf";

            if (!File.Exists(path))
                throw new Exception("Config file " + path + " does not exist!");

            using (StreamReader sr = new StreamReader(path))
            {
                string json = sr.ReadToEnd();

                try
                {
                    options = JObject.Parse(json);
                }
                catch (Exception e)
                {
                }
            }
        }

        public static string lookup(string key)
        {
            if (!loaded)
                load();


            if (!options.ContainsKey(key))
                throw new Exception("The key '" + key + "' was not set in the config: " + Directory.GetCurrentDirectory().ToString());
            return options[key].ToString();
        }

        public static string lookup_or_default(string key)
        {
            if (!loaded)
                load();

            if (!options.ContainsKey(key))
                return "";
            return options[key].ToString();
        }

        public static bool haskey(string key)
        {
            if (!loaded)
                load();
            return options.ContainsKey(key);
        }
    }
}