namespace ParsingPlaylists.Models.ResponseModels;

public class PlaylistVideoRenderer
{
    public string VideoId { get; set; }
    public TextModel Title { get; set; }
    public TextModel ShortBylineText { get; set; }
    public TextRun LengthText { get; set; }
}