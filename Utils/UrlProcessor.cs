using System.Diagnostics;

namespace ParsingPlaylists.Utils;

public static class UrlProcessor
{
    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}