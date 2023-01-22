using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingPlaylists.Models
{
    public class Playlist
    {
        public Playlist(string playlistName, string playlistDescription)
        {
            Name = playlistName;
            Description = playlistDescription;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Avatar { get; set; }

    }
}
