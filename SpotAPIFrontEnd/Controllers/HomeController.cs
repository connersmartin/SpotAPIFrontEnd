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
using System.Text;

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

        [HttpGet]
        public async Task<IActionResult> GetTracks(string id)
        {
            //get auth cookie
            HttpContext.Request.Cookies.TryGetValue("spotauthtoke", out string auth);
            var trackList = new List<TrackResponse>();

            var jsonParams = JsonSerializer.Serialize(new Dictionary<string, string>() { { "id", id }  });
            //make API call to get tracks for a specific playlist
            //var res = await _sas.Access("post", auth, "/Tracks", jsonParams).ConfigureAwait(true);

            var res = "[{\"artists\":\"Tito Puente\",\"title\":\"Salsa Y Sabor\",\"length\":181093},{\"artists\":\"Eddie Palmieri\",\"title\":\"Condiciones Que Existen\",\"length\":245666},{\"artists\":\"CNCO\",\"title\":\"Hey DJ\",\"length\":206920},{\"artists\":\"The Skatalites\",\"title\":\"The Guns of Navarone\",\"length\":407293},{\"artists\":\"Voodoo Glow Skulls\",\"title\":\"El Coo Cooi\",\"length\":157306},{\"artists\":\"Dasoul\",\"title\":\"Kung Fu\",\"length\":234280},{\"artists\":\"The Upsetters\",\"title\":\"Return of Django\",\"length\":149453},{\"artists\":\"Transplants\",\"title\":\"What I Can't Describe\",\"length\":242088},{\"artists\":\"Big D and the Kids Table\",\"title\":\"Shining On\",\"length\":194093},{\"artists\":\"The Skatalites\",\"title\":\"James Bond\",\"length\":187946},{\"artists\":\"Willie Bobo\",\"title\":\"Dig My Feeling\",\"length\":220198},{\"artists\":\"Ana Guerra\",\"title\":\"Ni La Hora\",\"length\":198406},{\"artists\":\"Harry J Allstars\",\"title\":\"Liquidator\",\"length\":172520},{\"artists\":\"KAROL G\",\"title\":\"Hello\",\"length\":195040},{\"artists\":\"Natti Natasha\",\"title\":\"Criminal\",\"length\":232549},{\"artists\":\"The Skatalites\",\"title\":\"Fugitive Dub\",\"length\":199552},{\"artists\":\"Felipe Peláez\",\"title\":\"Vivo Pensando En Ti (feat. Maluma)\",\"length\":232560},{\"artists\":\"Maluma\",\"title\":\"Felices los 4 (feat. Marc Anthony) - Salsa Version\",\"length\":242373},{\"artists\":\"Sonora Carruseles\",\"title\":\"Arranca en Fá\",\"length\":216120},{\"artists\":\"Voodoo Glow Skulls\",\"title\":\"Shoot the Moon\",\"length\":193146},{\"artists\":\"Ozuna\",\"title\":\"Vaina Loca\",\"length\":176133},{\"artists\":\"Mad Caddies\",\"title\":\"... and We Thought That Nation-States Were a Bad Idea\",\"length\":194658},{\"artists\":\"Luis Fonsi\",\"title\":\"Imposible\",\"length\":163880},{\"artists\":\"The Skatalites\",\"title\":\"Twelve Minutes To Go\",\"length\":186000},{\"artists\":\"Manu Chao\",\"title\":\"Me Gustas Tu\",\"length\":239986},{\"artists\":\"Don Omar\",\"title\":\"Te Quiero Pa´Mi\",\"length\":211626},{\"artists\":\"Streetlight Manifesto\",\"title\":\"Point / Counterpoint\",\"length\":327920},{\"artists\":\"Mad Caddies\",\"title\":\"State of Mind\",\"length\":226506},{\"artists\":\"Yandel\",\"title\":\"Mi Religión\",\"length\":237613},{\"artists\":\"The Skatalites\",\"title\":\"River Bank\",\"length\":327706},{\"artists\":\"Khea\",\"title\":\"Loca - Remix\",\"length\":346460},{\"artists\":\"Mongo Santamaria\",\"title\":\"Linda Guajira\",\"length\":183800},{\"artists\":\"Cachao\",\"title\":\"Oye Mis Tres Montunos\",\"length\":164812},{\"artists\":\"Bad Manners\",\"title\":\"Special Brew\",\"length\":135320},{\"artists\":\"Less Than Jake\",\"title\":\"The Science of Selling Yourself Short\",\"length\":186266},{\"artists\":\"Gerardo Ortiz\",\"title\":\"Recordando a Manuel\",\"length\":214453},{\"artists\":\"The Skatalites\",\"title\":\"Street Corner\",\"length\":184973},{\"artists\":\"Prince Royce\",\"title\":\"Deja vu\",\"length\":196480},{\"artists\":\"Poncho Sanchez\",\"title\":\"Bésame Mama\",\"length\":393106},{\"artists\":\"Big D and the Kids Table\",\"title\":\"Not Fuckin' Around\",\"length\":214133},{\"artists\":\"Mad Caddies\",\"title\":\"Sorrow\",\"length\":208730},{\"artists\":\"Sebastian Yatra\",\"title\":\"Traicionera - Remix\",\"length\":209320},{\"artists\":\"Eddie Palmieri\",\"title\":\"Vámonos Pa'l Monte\",\"length\":426840}]";


            //get the response and be able to return a partial view NOT WORKING
            var tracksResponse = JsonSerializer.Deserialize<TrackResponse[]>(res, null);
            //returns okay response with a redirect to viewing the tracks?

            return PartialView("ViewTracks", tracksResponse.Track);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylists()
        {
            //get auth cookie
            HttpContext.Request.Cookies.TryGetValue("spotauthtoke", out string auth);
            var playlistList = new List<PlaylistResponse>();

            //make api call to get all playlists
            var res = await _sas.Access("get", auth, "/Playlist", null).ConfigureAwait(true);
            //get the response and be able to return a partial view
            var playlistResponse = JsonSerializer.Deserialize<List<PlaylistResponse>>(res, null);
            //returns okay response with a redirect to viewing the tracks?

            return PartialView("ViewPlaylists", playlistResponse);
        }

        [HttpGet]
        public async Task<IActionResult> SpotParams()
        {
            //get auth cookie
            HttpContext.Request.Cookies.TryGetValue("spotauthtoke", out string auth);

            var genres = await GenresToArray(auth).ConfigureAwait(true);
            ViewData["genres"] = ArrayToSelectList(genres);
            ViewData["dance"] = DecimalToSelectList();
            ViewData["energy"] = DecimalToSelectList();
            ViewData["instrumental"] = DecimalToSelectList();
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> SpotParams([FromBody] CreatePlaylistRequest spotParams)
        {
            //get auth cookie
            HttpContext.Request.Cookies.TryGetValue("spotauthtoke", out string auth);

            spotParams.Tempo = spotParams.Tempo.Trim() == "" ? null : spotParams.Tempo;
            spotParams.Dance = spotParams.Dance.Trim() == "" ? null : spotParams.Dance;
            spotParams.Energy = spotParams.Energy.Trim() == "" ? null : spotParams.Energy;
            spotParams.Instrumental = spotParams.Instrumental.Trim() == "" ? null : spotParams.Instrumental;

            //jsonify params
            var jsonParams = JsonSerializer.Serialize(spotParams);
            //send to spot api
            var res = await _sas.Access("post", auth, "/Create", jsonParams).ConfigureAwait(true);
            //get the response and be able to return a partial view
            var playlistResponse = JsonSerializer.Deserialize<PlaylistResponse>(res, null);
            //returns okay response with a redirect to viewing the tracks?
            return PartialView("ViewPlaylists", new List<PlaylistResponse>() { playlistResponse });
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

        public List<SelectListItem> ArrayToSelectList(string[] items)
        {
            var list = new List<SelectListItem>();

            foreach (var t in items)
            {
                list.Add(new SelectListItem(t,t));
            }

            return list;
        }

        public static List<SelectListItem> DecimalToSelectList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem("n/a", ""));
            for (var i = 0.0m; i<=1; i+=0.1m)
            {
                list.Add(new SelectListItem(i.ToString(), i.ToString()));
            }

            return list;
        }
    }
}
