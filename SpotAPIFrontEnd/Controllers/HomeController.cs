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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;

namespace SpotAPIFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly SpotApiService _sas;
        public HomeController(IMemoryCache cache, ILogger<HomeController> logger, IConfiguration config, SpotApiService sas)
        {
            _cache = cache;
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
                baseUrl += "&scope=playlist-read-collaborative playlist-modify-public playlist-read-private playlist-modify-private user-read-email";
                var baseUri = new Uri(baseUrl);
                response = await client.GetAsync(baseUri).ConfigureAwait(true);                
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
                    var baseUrl = new Uri("https://accounts.spotify.com/api/token");
                    var body = new Dictionary<string, string>()
                    {
                        { "grant_type","authorization_code" },
                        { "code", Request.Query["code"].ToString() },
                        {"redirect_uri",  _config.GetValue<string>("redirectUri") },
                        {"client_id",  _config.GetValue<string>("clientId")},
                        {"client_secret",  _config.GetValue<string>("clientSecret") }
                    };
                    response = await client.PostAsync(baseUrl,new FormUrlEncodedContent(body)).ConfigureAwait(true);
                    responseContent = response.Content.ReadAsStringAsync().Result;                    
                }
                var tokenCookie = JsonSerializer.Deserialize<Token>(responseContent);
                HttpContext.Response.Cookies.Append("spotauthtoke", tokenCookie.access_token);

                //why not set genres now?
                var setGenres = await GenresToArray(tokenCookie.access_token).ConfigureAwait(true);
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
            var res = await _sas.Access("post", auth, "/Create", jsonParams).ConfigureAwait(true);
            //returns okay response with a redirect to viewing the tracks?
            return new JsonResult(new CreatePlaylistRequest { });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<string[]> GenresToArray(string auth)
        {
            var spotifyGenres = new GenreResponse();
            if (!_cache.TryGetValue("spotifyGenres", out spotifyGenres))
            {
                var baseAddress = "https://api.spotify.com/v1/recommendations/available-genre-seeds";
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth);

                    client.BaseAddress = new Uri(baseAddress);
                    var genreResponse = await client.GetAsync(baseAddress);
                    spotifyGenres = JsonSerializer.Deserialize<GenreResponse>(await genreResponse.Content.ReadAsByteArrayAsync(), null);

                }


                _cache.Set("spotifyGenres", spotifyGenres);

            }



            return spotifyGenres.genres;
        }
    }
}
