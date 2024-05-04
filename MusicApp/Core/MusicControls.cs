using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using MusicApp.API;
using NAudio;
using NAudio.Utils;
using NAudio.Wave;
namespace MusicApp.Core
{
    class MusicControls
    {
        public AudioFileReader? audioFile;
        public WaveOut waveOut = new WaveOut();
        public string? currentMusicPath;
        public Image? currentMusicImage;
        public Button? currentMusicButton;
        public bool isUserPaused = true,isMuted = false,isMusicSliding = false;
        public int currentMusicIndex;
        public float lastVolume;
        public void PlayMusic(string path,Label remainLabel)
        {
            waveOut.Dispose();
            waveOut = new WaveOut();
            audioFile = new AudioFileReader(path);
            waveOut.DesiredLatency = 1000;
            waveOut.Init(audioFile);
            waveOut.Play();
            remainLabel.Content = $"{audioFile.TotalTime.Minutes}:{audioFile.TotalTime.Seconds.ToString("00")}";
            currentMusicPath = path;
            isUserPaused = false;
        }
        public void PauseMusic()
        {
            waveOut.Pause();
            isUserPaused = true;
        }
        public void ResumeMusic()
        {
            waveOut.Resume();
            isUserPaused = false;
        }
        public float CalculatePercentage()
        {
            if(audioFile != null)
            {
                return (float)(audioFile.CurrentTime / audioFile.TotalTime * 100);
            }
            return 0;
        }
        public string CalculateAndFormatElapsedTime()
        {
            string minutes = audioFile.CurrentTime.Minutes.ToString();
            string seconds = audioFile.CurrentTime.Seconds.ToString("00");
            return $"{minutes}:{seconds}";
        }
        public string CalculatePreviewElapsedTime(double previewPercentage)
        {
            if(audioFile != null && waveOut != null) 
            {
                double x = previewPercentage / 100;
                double sec = audioFile.TotalTime.Minutes * 60 + audioFile.TotalTime.Seconds;
                double previewSeconds = x * sec;

                string formattedPreview = ((int)(previewSeconds / 60)).ToString() + ":" + ((int)(previewSeconds % 60)).ToString("00");
                return formattedPreview;
            }
            return null;
        }
    }
    class VolumeIcons
    {
        public const string Muted = "/mutedmusic-no-outline.png";
        public const string LowVolume = "/lowmusic.png";
        public const string MediumVolume = "/mediummusic.png";
        public const string HighVolume = "/highmusic.png";
    }

}
