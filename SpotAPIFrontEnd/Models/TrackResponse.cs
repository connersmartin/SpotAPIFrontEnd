using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotAPIFrontEnd.Models
{
    public class TrackResponse
    {
        public List<string> Artists { get; set; }
        public string Title { get; set; }
        public int Length { get; set; }
   
    }
}
