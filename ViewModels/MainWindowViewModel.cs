using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
using ParsingPlaylists.Models;
using ParsingPlaylists.Models.ResponseModels;
using ParsingPlaylists.Utils;
using ParsingPlaylists.Utils.Extensions;

namespace ParsingPlaylist.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Constants

        private const string YouTubeVideoLinkFormat = "https://www.youtube.com/watch?v={0}";
        private const string JsonRegexPattern =  @"(\{.*}\})";

        #endregion

        #region Fields

        private static readonly HtmlWeb Web;
        private Song _selectedItem;
        private List<string> _urls = new()
        {
            "https://www.youtube.com/playlist?list=PLWBAinm2sYBhADg8abuCctFfF4jTcgOPn",
            "https://www.youtube.com/playlist?list=PLMLISUSjWLb2QLHwHc6Pqn1piM1dRRPTk",
            "https://music.youtube.com/playlist?list=OLAK5uy_lWmVhD1gINd1Mp_9K7xl_hMg8gOLxLj58",
        };

        #endregion

        #region Properties

        public List<Song> Songs { get; set; }
        public Song SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();

                UrlProcessor.OpenUrl(value.URL);
            }
        }

        #endregion
        
        static MainWindowViewModel()
        {
            Web = new();
        }
        
        public MainWindowViewModel()
        {
            Songs = new();
            _urls.ForEach(Initialize);
        }

        public void Initialize(string url)
        {
            var playlistUrl = url.EscapeYtMusic();

            try
            {
                Songs.AddRange(GetPlaylistTracks(playlistUrl));
            }
            catch
            {
                MessageBoxManager
                    .GetMessageBoxStandardWindow("Error", $"Wrong url: {url}", icon: Icon.Error)
                    .Show();
            }
        }

        private static ICollection<Song> GetPlaylistTracks(string playlistUrl)
        {
            var songs = new List<Song>();
            
            var data = GetPlaylistInfo(playlistUrl);
          
            var playlist = new Playlist(
                playlistName: data.Header.PlaylistHeaderRenderer.Title.SimpleText,
                playlistDescription: data.Header.PlaylistHeaderRenderer.DescriptionText?.SimpleText ?? string.Empty);

            var tracks = data.Contents.TwoColumnBrowseResultsRenderer.Tabs[0].TabRenderer.Content.SectionListRenderer
                .Contents[0].ItemSectionRenderer.Contents.FirstOrDefault(x => x.PlaylistVideoListRenderer != null)?.PlaylistVideoListRenderer.Contents
                .Where(x => x.PlaylistVideoRenderer != null)
                .Select(x => x.PlaylistVideoRenderer).ToList();

            foreach (var song in tracks)
            {
                songs.Add(new Song
                {
                    Id = song.VideoId,
                    Name = song.Title.Runs[0].Text,
                    Artist = song.ShortBylineText.Runs[0].Text,
                    Duration = song.LengthText.SimpleText,
                    Playlist = playlist,
                    URL = string.Format(YouTubeVideoLinkFormat, song.VideoId)
                });
            }

            return songs;
        }

        private static PlaylistResponse? GetPlaylistInfo(string playlistUrl)
        {
            var doc = Web.Load(playlistUrl);

            var infoScript = doc.DocumentNode.Descendants().First(x => x.InnerText.Contains("InitialData")).InnerText;
            var json = Regex.Match(infoScript, JsonRegexPattern).Groups[1].Value;

            return JsonConvert.DeserializeObject<PlaylistResponse>(json);
        }
    }
}
