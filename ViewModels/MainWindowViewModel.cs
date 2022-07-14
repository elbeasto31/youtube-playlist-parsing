using Avalonia;
using Avalonia.Controls.Selection;
using Avalonia.Platform;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ParsingPlaylists.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            //playlist initialization

            Songs = new ObservableCollection<Song>();

            Initialize("https://www.youtube.com/playlist?list=PLWBAinm2sYBhADg8abuCctFfF4jTcgOPn");
            Initialize("https://www.youtube.com/playlist?list=PLMLISUSjWLb2QLHwHc6Pqn1piM1dRRPTk");
            Initialize("https://music.youtube.com/playlist?list=OLAK5uy_lWmVhD1gINd1Mp_9K7xl_hMg8gOLxLj58");
        }

        private IEnumerable<Song> _songs;
        public IEnumerable<Song> Songs
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

        //selected song item
        private Song selectedItem;

        public Song SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();

                //redirects to song's youtube page on click
                Process.Start(new ProcessStartInfo
                {
                    FileName = value.URL,
                    UseShellExecute = true
                });
            }
        }

        private void Initialize(string url)
        {
            //converts youtube music link to ordinary youtube link if needed
            url = url.Replace("music.", String.Empty);

            //adds new songs from url to the main collection
            Songs = Songs.Concat(new ObservableCollection<Song>(getPlaylist(url)));

        }

        public static ICollection<Song> getPlaylist(string playlistURL)
        {

            ICollection<Song> songs = new List<Song>();

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument doc = htmlWeb.Load(playlistURL);

            //finding initial data on the page
            var jsonText = doc.DocumentNode.Descendants().First(x => x.InnerText.Contains("InitialData")).InnerText;

            //parsing string to json object and removing variable declaration elements with substring method
            var data = JObject.Parse("{" + jsonText.Substring(21, jsonText.Length - 22));

            var playlistName = data.SelectTokens("$..playlistHeaderRenderer.title.simpleText").FirstOrDefault();
            var playlistDescription = data.SelectTokens("$..playlistHeaderRenderer.descriptionText.simpleText").FirstOrDefault() ?? String.Empty;

            Playlist playlist = new Playlist { Name = playlistName.ToString(), Description = playlistDescription.ToString().Replace("\n", " ") };

            //finding json tokens that contain data about songs
            IEnumerable<JToken> playlistChildren = data.SelectTokens("$..playlistVideoListRenderer.contents").Children();

            foreach (var song in playlistChildren)
            {
                if (song.SelectToken("$..playlistVideoRenderer") != null)
                {
                    string title = song.SelectTokens("$..title.runs[0].text").First().ToString();
                    string artist = song.SelectTokens("$..shortBylineText.runs[0].text").First().ToString();
                    string duration = song.SelectTokens("$..lengthText.simpleText").First().ToString();
                    string url = "https://www.youtube.com/watch?v=" + song.SelectTokens("$..playlistVideoRenderer.videoId").First().ToString();

                    songs.Add(new Song { Name = title, Artist = artist, Duration = duration, Playlist = playlist, URL = url });
                }
            }

            return songs;
        }
    }
}
