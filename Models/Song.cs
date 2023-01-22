using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ParsingPlaylists.Models
{
    public class Song
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public string Duration { get; set; }

        public string URL { get; set; }

        [JsonIgnore]
        public Bitmap Image { get; set; }

        public Playlist Playlist { get; set; }
    }
}
