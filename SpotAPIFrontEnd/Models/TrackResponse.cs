using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotAPIFrontEnd.Models
{
    public class TrackResponse
    {
        [JsonProperty("artists")]
        public string artists { get; set; }
        [JsonProperty("title")]
        public string title { get; set; }
        [JsonProperty("length")] 
        public int length { get; set; }
   
    }
}
