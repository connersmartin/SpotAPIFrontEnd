using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Net.Http.Headers;

namespace SpotAPIFrontEnd.Services
{
    public class SpotApiService
    {
        private readonly ILogger<SpotApiService> _logger;
        private readonly IConfiguration _config;
        public SpotApiService(ILogger<SpotApiService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task<string> Access(string method,string path, string json)
        {
            var url = _config.GetValue<string>("spotServiceUrl");
            var response = new HttpResponseMessage();
                        
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                switch (method)
                {
                    case "get":
                        response = await client.GetAsync(url + path);
                        break;
                    case "post":
                        var postParams = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await client.PostAsync(url + path, postParams);
                        break;
                    default:
                        response = new HttpResponseMessage() { Content = new StringContent("Invalid method invoked") };
                        break;
                }
            }
            return await response.Content.ReadAsStringAsync();
        }
        
    }
}
