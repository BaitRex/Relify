using MusicApp.Core;
using System;
using System.IO;
using System.Net;
using TagLib;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
namespace MusicApp.API
{
    public class Network
    {
        #region Search
        public async Task<List<Video>> Search(string query, string resultCount)
        {
            var client = new YoutubeClient(new List<Cookie>());
            int count = 0;
            List<Music> currentMusics = PathWorking.FetchMusics();
            List<Video> videos = new List<Video>();
            await foreach (var video in client.Search.GetVideosAsync(query.ToString()))
            {
                count = count + 1;


                string title = video.Title;
                string thumbnail = video.Thumbnails[0].Url;
                string id = video.Id;
                string icon;
                bool isAlreadyDownloaded = currentMusics.Any(item => item.ID == id);

                
                
                if (isAlreadyDownloaded) { icon = DownloadIcons.DownloadFinishedIcon; }
                else { icon = DownloadIcons.DownloadIcon; }

                
                Console.WriteLine($"Video Title: {title}");
                Console.WriteLine($"Video Title: {thumbnail}");
                Console.WriteLine($"Video Title: {id}");
                Console.WriteLine($"Count : {count}");
                Console.WriteLine("\n\n");

                // Video nesneleri oluşturup videos listesine eklemek istiyorsanız:
                videos.Add(new Video
                {
                    Thumbnail = thumbnail,
                    Title = title,
                    VideoID = id,
                    DownloadIcon = icon
                });

                    
                
                if (count > Convert.ToInt16(resultCount) - 1)
                        break;
                
            }


            return videos;


            #region Old 
            /*string url = $"http://172.214.90.27:5000/search?searchQuery={query}&resultsCount={resultCount}";

            List<Music> currentMusics = PathWorking.FetchMusics();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync(); // Veriyi döndür
                    List<Video> videos = new List<Video>();
                    var root = JsonConvert.DeserializeObject<dynamic>(result); // JSON verisini deserialize ediyor

                    var videosArray = root["videos"]; // "videos" dizisine eriş

                    foreach (var video in videosArray)
                    {
                        string title = video["title"];
                        string thumbnail = video["thumbnail"];
                        string id = video["videoID"];
                        string icon;
                        bool isAlreadyDownloaded = currentMusics.Any(item => item.ID == id);

                        if (isAlreadyDownloaded) { icon = DownloadIcons.DownloadFinishedIcon; }
                        else { icon = DownloadIcons.DownloadIcon; }

                        Console.WriteLine($"Video Title: {title}");
                        Console.WriteLine($"Video Title: {thumbnail}");
                        Console.WriteLine($"Video Title: {id}");
                        Console.WriteLine("\n\n");

                        // Video nesneleri oluşturup videos listesine eklemek istiyorsanız:
                        videos.Add(new Video
                        {
                            Thumbnail = video["thumbnail"],
                            Title = video["title"],
                            VideoID = video["videoID"],
                            DownloadIcon = icon
                        });
                    }

                    return videos;
                }
                else
                {
                    string messageBoxText = $"We didn't connect the api... {response.StatusCode}";
                    string caption = "Something vent wrong!";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                    return null;
                }
            }*/
            #endregion
        }



        #endregion

        #region Download

        
        

        public async Task GetDownloadURL(string videoid,string savePath,string thumbnail,string title)
        {
            var youtube = new YoutubeClient();

            // Videoyu alın
            var videoUrl = $"https://youtube.com/watch?v={videoid}";
            var video = await youtube.Videos.Streams.GetManifestAsync(videoUrl);
            using (WebClient client = new WebClient())
            {
                byte[] pictArray = await client.DownloadDataTaskAsync(thumbnail);
                // Sadece ses akışını alın
                var audioStreamInfo = video.GetAudioOnlyStreams().GetWithHighestBitrate();

                // Download the stream to a file
                await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, $"{savePath}{title.Normalize()}.webm");
                await PathWorking.Convert.WEBM_to_MP3($"{savePath}{title}.webm", $"{savePath}{title}.mp3");


                var file = TagLib.File.Create($"{savePath}{title}.mp3");
                file.Tag.Pictures = [new Picture(new ByteVector(pictArray))];
                file.Tag.Album = videoid;
                file.Save();
            }
            Console.WriteLine("Dosya indirildi.");
        }


        #region Old 
        public static async void DownloadOnURL(string url, string savePath, string thumbnail, string videoID)
        {

            using (WebClient client = new WebClient())
            {
                try
                {

                    client.DownloadFile(url, savePath); // Dosyayı indir ve belirtilen yola kaydet
                    var file = TagLib.File.Create(savePath);
                    byte[] picture = client.DownloadData(thumbnail);
                    file.Tag.Pictures = [new Picture(new ByteVector(picture))];
                    file.Tag.Album = videoID;
                    file.Save();

                    Console.WriteLine("Dosya indirildi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message + "\n" + ex.InnerException);
                }
            }

        }
        #endregion

        #endregion
    }

    public class DownloadIcons
    {
        public const string DownloadIcon = "/MVVM/View/download.png";
        public const string LoadingIcon = "/MVVM/View/loading-cropped.gif";
        public const string DownloadFinishedIcon = "/MVVM/View/download-finished.png";
    }
}
