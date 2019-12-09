using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotAPIFrontEnd.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SpotAPIFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {     

            return View();
                        
        }

        public async Task<RedirectResult> Auth()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var baseUrl = "https://accounts.spotify.com/authorize";
                baseUrl += "?client_id=" + _config.GetValue<string>("clientId");
                baseUrl += "&response_type=code";
                baseUrl += "&redirect_uri=" + _config.GetValue<string>("redirectUri");
                baseUrl += "&scope=playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private";
                response = await client.GetAsync(baseUrl);                
            }
            return Redirect(response.RequestMessage.RequestUri.ToString());
        }


        public async Task<IActionResult> CodeToken()
        {
            if (Request.Query.ContainsKey("code"))
            {
                HttpResponseMessage response = new HttpResponseMessage();
                var responseContent ="";
                using (var client = new HttpClient())
                {
                    var baseUrl = "https://accounts.spotify.com/api/token";
                    var body = new Dictionary<string, string>()
                    {
                        { "grant_type","authorization_code" },
                        { "code", Request.Query["code"].ToString() },
                        {"redirect_uri",  _config.GetValue<string>("redirectUri") },
                        {"client_id",  _config.GetValue<string>("clientId")},
                        {"client_secret",  _config.GetValue<string>("clientSecret") }
                    };
                    response = await client.PostAsync(baseUrl,new FormUrlEncodedContent(body));
                    responseContent = response.Content.ReadAsStringAsync().Result.ToString();                    
                }
                var tokenCookie = JsonSerializer.Deserialize<Token>(responseContent);
                HttpContext.Response.Cookies.Append("spotauthtoke", tokenCookie.access_token);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<JsonResult> SpotParams(Spot spotParams)
        {
           //get auth cookie
           //jsonify params
           //send to spot api

           //returns okay response with a redirect to viewing the tracks?
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
