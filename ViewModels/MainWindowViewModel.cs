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


        public Playlist Playlist { get; set; }
        /// public static Bitmap im { get; set; }/

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

        private int currentIndex;

        public int CurrentIndex
        {
            get { return currentIndex; }

            set
            {
                if (currentIndex == value)
                    return;

                currentIndex = value;
                OnPropertyChanged();
            }
        }



        private void Initialize(string url)
        {
            Playlist temp;
            Songs = new ObservableCollection<Song>(getPlaylist(url, out temp));
            Playlist = temp;
        }



        public static List<Song> getPlaylist(string playlistURL, out Playlist playlist)
        {

            List<Song> songs = new List<Song>();

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument doc = htmlWeb.Load(playlistURL);


            var jsonText = doc.DocumentNode.SelectSingleNode("/html/body/script[15]").InnerText.Substring(21).Replace(';'.ToString(), String.Empty);
            var data = JObject.Parse("{" + jsonText);



            string playlistName = data.SelectTokens("$..title.simpleText").First().ToString();
            string playlistDescription = data.SelectTokens("$..playlistHeaderRenderer.descriptionText").First().ToString();

            playlist = new Playlist { Name = playlistName, Description = playlistDescription };

            IEnumerable<JToken> playlistChildren = data.SelectTokens("$..playlistVideoListRenderer.contents").Children();

            foreach (var song in playlistChildren)
            {
                if (song.Next != null)
                {
                    string imageURL = song.SelectTokens("$..thumbnails[0].url").Last().ToString();

                    //WebClient wc = new WebClient();
                    //byte[] originalData = wc.DownloadData(imageURL);

                    //MemoryStream stream = new MemoryStream(originalData);
                    //Bitmap songPicture = new Bitmap(stream);

                    //FileStream stream;
                    //using (stream = System.IO.File.Open("E:\\Images\\32b40a2a259d9e4229a18b6d66e89c33.jpg", FileMode.Open))
                    //{
                    //    songPicture = new Bitmap(stream);
                    //}



                    string title = song.SelectTokens("$..title.runs[0].text").First().ToString();
                    string artist = song.SelectTokens("$..shortBylineText.runs[0].text").First().ToString();
                    string duration = song.SelectTokens("$..lengthText.simpleText").First().ToString();
                    songs.Add(new Song {  Name = title, Artist = artist, Duration = duration, Playlist = playlist });
                }
            }

            return songs;
        }



        //private static void DownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        //{
        //    try
        //    {
        //        byte[] bytes = e.Result;

        //        Stream stream = new MemoryStream(bytes);

        //        var image = new Bitmap(stream);
        //        im = image;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex);
        //        im = null;
        //    }

        //}
    }
}
