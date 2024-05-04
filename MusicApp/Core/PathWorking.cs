using MusicApp.API;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace MusicApp.Core
{
    class PathWorking
    {
        #region Check Folders
        public static void CheckFolders()
        {
            string musicFolderDIRECTORY = Directory.GetCurrentDirectory() + @"\Musics";
            bool isMusicFolderOK = Path.Exists(musicFolderDIRECTORY);

            string playlistFolderDIRECTORY = Directory.GetCurrentDirectory() + @"\Playlists";
            bool isPlaylistFolderOK = Path.Exists(playlistFolderDIRECTORY);

            if (!isMusicFolderOK)
            {
                Directory.CreateDirectory(musicFolderDIRECTORY);
            }
            if (!isPlaylistFolderOK)
            {
                Directory.CreateDirectory(playlistFolderDIRECTORY);
            }
        }
        #endregion

        #region Fetch Musics 
        public static List<Music> FetchMusics()
        {
            string folderPath = Directory.GetCurrentDirectory() + @"\Musics"; // Klasör yolunu belirtin
            List<Music> musics = new();
            // Klasördeki tüm dosyaları alın
            string[] files = Directory.GetFiles(folderPath);

            // Dosyaları ekrana yazdırın
            Console.WriteLine("Klasördeki dosyalar:");
            foreach (string file in files)
            {
                try
                {
                    var music = TagLib.File.Create(Path.GetFullPath(file));

                    MemoryStream ms = new MemoryStream(music.Tag.Pictures[0].Data.Data);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();



                    musics.Add(new Music
                    {
                        Title = Path.GetFileNameWithoutExtension(file),
                        MusicPath = Path.GetFullPath(file),
                        Thumbnail = bitmap,
                        ID = music.Tag.Album
                    });
                    Console.WriteLine(file);
                }
                catch
                {
                    try
                    {
                        File.Delete(Path.GetFullPath(file));
                    }
                    catch
                    {
                        Console.WriteLine("What the fuck");
                    }
                }
            }
            return musics;
        }

        #endregion

        public class Convert
        {
            #region Convertion



            #region WEBM to MP3

            public static async Task WEBM_to_MP3(string inputFile,string outputFile)
            {
                // FFmpeg komutunu oluştur
                string ffmpegPath = "ffmpeg.exe"; // FFmpeg'in yolunu buraya girin
                string arguments = $"-i \"{inputFile}\" -vn -acodec libmp3lame -q:a 4 \"{outputFile}\"";

                // FFmpeg'i çalıştır
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);
                    process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }

                File.Delete(inputFile);

                Console.WriteLine("Dönüştürme tamamlandı.");
            }

            #endregion


            #endregion
        }


    }
}
