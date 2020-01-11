using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpotAPIFrontEnd.Models
{
    public class PlaylistResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("trackCount")]
        public int TrackCount { get; set; }
        [JsonPropertyName("length")]
        public int Length { get; set; }       

    }
}
