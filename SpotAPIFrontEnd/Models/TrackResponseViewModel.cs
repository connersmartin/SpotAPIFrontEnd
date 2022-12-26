using System.Collections.Generic;

namespace SpotAPIFrontEnd.Models
{
    public class TrackResponseViewModel
    {
        public string Id { get; set; }
        public string Name { get;set; }
        public List<TrackViewModel> Tracks {get;set;}
    }
}
