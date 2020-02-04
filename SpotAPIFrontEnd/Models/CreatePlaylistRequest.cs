using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpotAPIFrontEnd.Models
{
    public class CreatePlaylistRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Length { get; set; }
        public string[] Genres { get; set; }
        public string Artist { get; set; }
        public string Tempo { get; set; }
        public string Dance { get; set; }
        public string Energy { get; set; }
        public string Valence { get; set; }

    }
}
