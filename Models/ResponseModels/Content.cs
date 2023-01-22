namespace ParsingPlaylists.Models.ResponseModels;

public class Content
{
    public SectionListRenderer SectionListRenderer { get; set; }
    public ItemSectionRenderer ItemSectionRenderer { get; set; }
    public PlaylistVideoListRenderer PlaylistVideoListRenderer { get; set; }
    public PlaylistVideoRenderer PlaylistVideoRenderer { get; set; }
}