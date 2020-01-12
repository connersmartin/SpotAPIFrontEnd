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
        public string Artists { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("length")] 
        public int Length { get; set; }
   
    }
}
