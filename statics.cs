using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;

namespace ValidityAPI
{
    public static class statics
    {
       

        public static RPC_Credentials creds = new  RPC_Credentials(ConfigFileReader.lookup("rpc_user"), ConfigFileReader.lookup("rpc_pass"), ConfigFileReader.lookup("rpc_ip"), int.Parse(ConfigFileReader.lookup("rpc_port")));
        public static BitnetClient BC = new BitnetClient(creds);

        public static JArray response = new JArray();

        public static Timer timer = new Timer(Update, new AutoResetEvent(false), 1000, 30000);



        public static decimal circ_supply = 0;
        public static Market bittrex_val_btc = new Market();
        public static Market upbit_val_btc = new Market();

        public static Market bittrex_btc_usd = new Market();
        public static Market bittrex_val_usdt = new Market();


        public static Market upbit_krw_btc = new Market();
        public static Market upbit_idr_btc = new Market();
        public static Market upbit_sdg_btc = new Market();
        public static Market upbit_thb_btc = new Market();

        public static void Update(Object stateInfo)
        {
            try
            {
                JObject base_info = new JObject();

                circ_supply = decimal.Parse(BC.gettxoutsetinfo()["total_amount"].ToString());

                base_info.Add("symbol", "VAL");
                base_info.Add("circulatingSupply", circ_supply);
                base_info.Add("maxSupply", 9000000);
                base_info.Add("provider", "ValidityTech.com");
                base_info.Add("lastUpdatedTimestamp", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());

                using (HttpClient client = new HttpClient())
                {

                    bittrex_btc_usd.bittrex_parse(JObject.Parse(client.GetStringAsync("https://api.bittrex.com/api/v1.1/public/getmarketsummary?market=USD-BTC").Result));
                    bittrex_val_btc.bittrex_parse(JObject.Parse(client.GetStringAsync("https://api.bittrex.com/api/v1.1/public/getmarketsummary?market=btc-val").Result));




                    upbit_val_btc.upbit_parse((JObject)JArray.Parse(client.GetStringAsync("https://api.upbit.com/v1/ticker?markets=BTC-VAL").Result).First);


                    upbit_krw_btc.upbit_parse((JObject)JArray.Parse(client.GetStringAsync("https://api.upbit.com/v1/ticker?markets=krw-BTC").Result).First);

                    //krw
                    upbit_krw_btc.upbit_parse((JObject)JArray.Parse(client.GetStringAsync("https://api.upbit.com/v1/ticker?markets=krw-BTC").Result).First);
                    upbit_idr_btc.upbit_parse((JObject)JArray.Parse(client.GetStringAsync("https://id-api.upbit.com/v1/ticker?markets=idr-BTC").Result).First);
                    upbit_sdg_btc.upbit_parse((JObject)JArray.Parse(client.GetStringAsync("https://sg-api.upbit.com/v1/ticker?markets=sgd-BTC").Result).First);
                    upbit_thb_btc.upbit_parse((JObject)JArray.Parse(client.GetStringAsync("https://th-api.upbit.com/v1/ticker?markets=thb-BTC").Result).First);

                }

                decimal avg_val_price = Math.Round((upbit_val_btc.lastTradeRate + bittrex_val_btc.lastTradeRate) / 2, 8);







                JObject usd = new JObject(base_info);
                usd.Add("currencyCode", "USD");
                usd.Add("price", Math.Round(avg_val_price * bittrex_btc_usd.lastTradeRate, 8));
                usd.Add("marketCap", Math.Round(avg_val_price * bittrex_btc_usd.lastTradeRate * circ_supply, 2));
                usd.Add("accTradePrice24h", "");

                JObject BTC = new JObject(base_info);
                BTC.Add("currencyCode", "BTC");
                BTC.Add("price", avg_val_price);
                BTC.Add("marketCap", Math.Round(avg_val_price * circ_supply, 2));
                BTC.Add("accTradePrice24h", "");

                JObject krw = new JObject(base_info);
                krw.Add("currencyCode", "KRW");
                krw.Add("price", Math.Round(avg_val_price * upbit_krw_btc.lastTradeRate, 2));
                krw.Add("marketCap", Math.Round(avg_val_price * upbit_krw_btc.lastTradeRate * circ_supply, 2));
                krw.Add("accTradePrice24h", "");

                JObject idr = new JObject(base_info);
                idr.Add("currencyCode", "IDR");
                idr.Add("price", Math.Round(avg_val_price * upbit_idr_btc.lastTradeRate, 2));
                idr.Add("marketCap", Math.Round(avg_val_price * upbit_idr_btc.lastTradeRate * circ_supply, 2));
                idr.Add("accTradePrice24h", "");

                JObject sgd = new JObject(base_info);
                sgd.Add("currencyCode", "SGD");
                sgd.Add("price", Math.Round(avg_val_price * upbit_sdg_btc.lastTradeRate, 2));
                sgd.Add("marketCap", Math.Round(avg_val_price * upbit_sdg_btc.lastTradeRate * circ_supply, 2));
                sgd.Add("accTradePrice24h", "");

                JObject thb = new JObject(base_info);
                thb.Add("currencyCode", "THB");
                thb.Add("price", Math.Round(avg_val_price * upbit_thb_btc.lastTradeRate, 2));
                thb.Add("marketCap", Math.Round(avg_val_price * upbit_thb_btc.lastTradeRate * circ_supply, 2));
                thb.Add("accTradePrice24h", "");


                response = new JArray();

                response.Add(usd);
                response.Add(BTC);
                response.Add(krw);
                response.Add(idr);
                response.Add(sgd);
                response.Add(thb);


            }
            catch
            {
                Console.WriteLine("update failed" + DateTime.Now.ToString());
            }



        }



        private static decimal getPrice(decimal coin, decimal fiat)
        {
            return Math.Round(coin * fiat, 2);
        }

       


    }

    public  class Market_data
    {
        decimal fiat_price;
        decimal market_cap;

        public Market_data(decimal coin, decimal fiat, decimal supply)
        {
            fiat_price = Math.Round(coin * fiat, 2);
            market_cap = Math.Round(fiat_price * fiat, 2);
        }

        
    }


    public class Market
    {


      

        public void upbit_parse(JObject data)
        {

            lastTradeRate = decimal.Parse(data["trade_price"].ToString());
            volume = Math.Round(decimal.Parse(data["acc_trade_volume_24h"].ToString()), 2);
           
        }

        public void bittrex_parse(JObject data)
        {

            volume = Math.Round(decimal.Parse(data["result"][0]["Volume"].ToString()), 2);
            lastTradeRate = decimal.Parse(data["result"][0]["Last"].ToString());
            PrevDay = decimal.Parse(data["result"][0]["PrevDay"].ToString());
            rate_change = Math.Round(100 * (lastTradeRate - PrevDay) / ((lastTradeRate + PrevDay) / 2), 1);

        }






        public decimal volume;
        public decimal lastTradeRate;
        public decimal PrevDay;

        public decimal price_change;
        public decimal rate_change;
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
