
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace ValidityAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class Front_controller : ControllerBase
    {
        private string listenIP = JArray.Parse(ConfigFileReader.lookup("listens"))[0]["listen_url"].ToString();

        [HttpGet]
        [Route("check")]
        public HttpStatusCode check()
        {
            return HttpStatusCode.OK;
        }

        [HttpGet]
        [Route("status")]
        public string status()
        {
            return "up. Version 'pw reset' ip: " + listenIP + " " + DateTime.Now;
        }

       
    }
}