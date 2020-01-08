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
        public decimal Tempo { get; set; }
        public decimal Dance { get; set; }
        public decimal Energy { get; set; }
        public decimal Instrumental { get; set; }

    }
}
