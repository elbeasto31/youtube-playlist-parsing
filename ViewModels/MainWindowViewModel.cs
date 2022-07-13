using Avalonia;
using Avalonia.Controls.Selection;
using Avalonia.Platform;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ParsingPlaylists.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ParsingPlaylists.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            Initialize("https://www.youtube.com/playlist?list=PLMLISUSjWLb2QLHwHc6Pqn1piM1dRRPTk");
        }

        private ObservableCollection<Song> _songs;
        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set
            {
                if (value != null)
                {
                    _songs = value;
                    OnPropertyChanged();
                }
            }
        }

        private void Initialize(string url)
        {
            Songs = new ObservableCollection<Song>(getPlaylist(url));
        }

        public static List<Song> getPlaylist(string playlistURL)
        {

            List<Song> songs = new List<Song>();

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument doc = htmlWeb.Load(playlistURL);

            //finding initial data on the page
            var jsonText = doc.DocumentNode.Descendants().First(x => x.InnerText.Contains("InitialData")).InnerText;

            //parsing string to json object and removing variable declaration elements with substring method
            var data = JObject.Parse("{" + jsonText.Substring(21, jsonText.Length - 22));

            string playlistName = data.SelectTokens("$..title.simpleText").First().ToString();
            string playlistDescription = data.SelectTokens("$..playlistHeaderRenderer.descriptionText").First().ToString();

            Playlist playlist = new Playlist { Name = playlistName, Description = playlistDescription };

            //finding json tokens that contain data about songs
            IEnumerable<JToken> playlistChildren = data.SelectTokens("$..playlistVideoListRenderer.contents").Children();

            foreach (var song in playlistChildren)
            {
                if (song.Next != null)
                {
                    string title = song.SelectTokens("$..title.runs[0].text").First().ToString();
                    string artist = song.SelectTokens("$..shortBylineText.runs[0].text").First().ToString();
                    string duration = song.SelectTokens("$..lengthText.simpleText").First().ToString();
                    songs.Add(new Song { Name = title, Artist = artist, Duration = duration, Playlist = playlist });
                }
            }

            return songs;
        }
    }
}
