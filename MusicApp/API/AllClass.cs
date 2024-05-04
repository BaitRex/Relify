using System;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace MusicApp.API
{
    public class Music
    {
        public BitmapImage? Thumbnail { get; set; }
        public string Title { get; set; }
        public string MusicPath { get; set; }

        public bool? isSelected { get; set; }
        public string? ID { get; set; }
    }
    public class Video
    {
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public string VideoID { get; set; }
        public string? DownloadIcon { get; set; }
    }
    public class Playlist
    {
        public string Title { get; set; }
        public List<Music> MusicsInPlaylist { get; set; }

    }
}
