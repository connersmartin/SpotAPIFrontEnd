using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotAPIFrontEnd.Models
{
    public class PlaylistResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int TrackCount { get; set; }
        public int Length { get; set; }       

    }
}
