using System;

namespace ParsingPlaylists.Utils.Extensions;

public static class StringExtensions
{
    private const string YouTubeMusicDomain = "music.youtube";

    public static string EscapeYtMusic(this string url)
    {
        if(url.Contains(YouTubeMusicDomain))
            return url.Replace("music.", String.Empty);

        return url;
    }
}