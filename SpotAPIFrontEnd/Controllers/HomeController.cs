using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotAPIFrontEnd.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using SpotAPIFrontEnd.Services;

namespace SpotAPIFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly SpotApiService _sas;
        public HomeController(ILogger<HomeController> logger, IConfiguration config, SpotApiService sas)
        {
            _logger = logger;
            _config = config;
            _sas = sas;
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
                HttpResponseMessage response;
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
        public async Task<JsonResult> SpotParams(CreatePlaylistRequest spotParams)
        {
            //get auth cookie
            HttpContext.Request.Cookies.TryGetValue("spotauthtoke", out string auth);

            //jsonify params
            var jsonParams = JsonSerializer.Serialize(spotParams);
            //send to spot api
            var res = await _sas.Access("post", auth, "/Create", jsonParams);
            //returns okay response with a redirect to viewing the tracks?
            return new JsonResult(new CreatePlaylistRequest { });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
