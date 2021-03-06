using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;

namespace ValidityAPI
{
    public static class stakingRewards
    {
        public static RPC_Credentials creds = new RPC_Credentials(ConfigFileReader.lookup("rpc_user"), ConfigFileReader.lookup("rpc_pass"), ConfigFileReader.lookup("rpc_ip"), int.Parse(ConfigFileReader.lookup("rpc_port")));
        public static BitnetClient BC = new BitnetClient(creds);

        public static JObject response = new JObject();

        public static Timer timer = new Timer(Update, new AutoResetEvent(false), 1000, 30000);
       



        public static void Update(Object stateInfo)
        {
            try {
                JObject new_data = new JObject();

                new_data = new JObject();
                new_data.Add("circulatingSupply", Math.Round(wallet.circ_supply, 2));
                new_data.Add("netstakeweight", wallet.netstakeweight);
                new_data.Add("blockcount", BC.GetBlockCount());
                new_data.Add("stakereturn", Math.Round(((.485 * 1440 * 365) / (double)wallet.netstakeweight) * 100, 2));
                response = new_data;


            }
            catch { }
            
        }

    }

   

    
}