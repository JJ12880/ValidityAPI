using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace ValidityAPI
{
    [ApiController]
    [Route("[controller]")]
    public class Api : ControllerBase
    {



        private string listenIP = JArray.Parse(ConfigFileReader.lookup("listens"))[0]["listen_url"].ToString();

        

        [HttpGet]
        [Route("status")]
        public string status()
        {
            return "up. Version 1 ip: " + listenIP + " " + DateTime.Now;
        }

        [HttpGet]
        [Route("check")]
        public HttpStatusCode check()
        {
            return HttpStatusCode.OK;
        }

        [HttpGet]
        [Route("val")]
        public JArray val(string id)
        {
          

            return statics.response;
        }
    }
}