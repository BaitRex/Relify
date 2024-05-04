using MusicApp.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MusicApp.API;
using System.IO;
using NAudio;
using System.Windows.Media.Imaging;
using NAudio.Wave;
using MusicApp.MVVM.ViewModel;
using System.Windows.Shapes;

namespace MusicApp.MVVM.View
{
    /// <summary>
    /// MusicView.xaml etkileşim mantığı
    /// </summary>
    public partial class MusicView : UserControl
    {
        MusicControls musicControl;
        MainViewModel mainViewModel;
        internal MusicView(MainViewModel viewModel)
        {
            musicControl = viewModel.MusicControl;
            mainViewModel = viewModel;
            InitializeComponent();
            dataGrid.ItemsSource = PathWorking.FetchMusics();
        }

        private void PlayMusic_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            DataGridCell cell = FindVisualParent<DataGridCell>(clickedButton);
            if (cell != null)
            {
                // Veri satırını bulma
                var music = (cell.DataContext as Music);
                if (music != null && cell != null)
                {
                    if(musicControl.waveOut.PlaybackState == PlaybackState.Stopped)
                    {
                        musicControl.PlayMusic(music.MusicPath,mainViewModel.MainWindow.totalTime);
                        musicControl.currentMusicIndex = FindInMusicList(mainViewModel.MainWindow.musicList,music);
                        ChangePlayButtonIcon("/MVVM/View/stopbutton.png", clickedButton, mainViewModel.MainWindow.playPauseMusicBtn);
                        mainViewModel.MainWindow.UpdateMainControlItems(music.Thumbnail.BaseUri, music.Title);
                        musicControl.currentMusicButton = clickedButton;
                        musicControl.currentMusicImage = mainViewModel.MainWindow.playPauseMusicBtn;

                    }
                    else if(musicControl.waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        if(musicControl.currentMusicPath == music.MusicPath)
                        {
                            musicControl.PauseMusic();
                            ChangePlayButtonIcon("/MVVM/View/playbutton.png", clickedButton, mainViewModel.MainWindow.playPauseMusicBtn);
                        }
                        else
                        {
                            musicControl.PlayMusic(music.MusicPath, mainViewModel.MainWindow.totalTime);
                            musicControl.currentMusicIndex = FindInMusicList(mainViewModel.MainWindow.musicList, music);
                            ChangePlayButtonIcon("/MVVM/View/playbutton.png", musicControl.currentMusicButton, mainViewModel.MainWindow.playPauseMusicBtn);
                            ChangePlayButtonIcon("/MVVM/View/stopbutton.png", clickedButton, mainViewModel.MainWindow.playPauseMusicBtn);
                            mainViewModel.MainWindow.UpdateMainControlItems(music.Thumbnail.BaseUri, music.Title);
                            musicControl.currentMusicButton = clickedButton;
                            musicControl.currentMusicImage = mainViewModel.MainWindow.playPauseMusicBtn;
                        }
                    }
                    else if(musicControl.waveOut.PlaybackState == PlaybackState.Paused)
                    {
                        if (musicControl.currentMusicPath == music.MusicPath)
                        {
                            musicControl.ResumeMusic();
                            musicControl.currentMusicIndex = mainViewModel.MainWindow.musicList.IndexOf(music);
                            ChangePlayButtonIcon("/MVVM/View/stopbutton.png", clickedButton, mainViewModel.MainWindow.playPauseMusicBtn);
                        }
                        else
                        {
                            musicControl.PlayMusic(music.MusicPath, mainViewModel.MainWindow.totalTime);
                            musicControl.currentMusicIndex = FindInMusicList(mainViewModel.MainWindow.musicList, music);
                            ChangePlayButtonIcon("/MVVM/View/playbutton.png", musicControl.currentMusicButton, mainViewModel.MainWindow.playPauseMusicBtn);
                            ChangePlayButtonIcon("/MVVM/View/stopbutton.png", clickedButton, mainViewModel.MainWindow.playPauseMusicBtn);
                            mainViewModel.MainWindow.UpdateMainControlItems(music.Thumbnail.BaseUri, music.Title);
                            musicControl.currentMusicButton = clickedButton;
                            musicControl.currentMusicImage = mainViewModel.MainWindow.playPauseMusicBtn;
                        }
                    }
                }
            }
        }
        internal int FindInMusicList(List<Music> list,Music item)
        {
            int index = -1;
            foreach(Music music in list)
            {
                index++;
                if(music.MusicPath == item.MusicPath) return index;
            }
            return index;
        }
        private void ChangePlayButtonIcon(string path,Button clickedButton,Image mainWindowIco)
        {
            Image playBtnIco = (Image)clickedButton.FindName("downloadImg");
            Uri imageUri = new Uri(path, UriKind.RelativeOrAbsolute);
            BitmapImage bitmap = new BitmapImage(imageUri);
            playBtnIco.Source = bitmap;
            mainWindowIco.Source = bitmap;
        }
        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            if (parentObject is T parent)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }
        private void PlayMusic_MouseEnter(object sender, MouseEventArgs e)
        {
            Button senderBtn = (Button)sender;
            senderBtn.Opacity = 1;
        }

        private void PlayMusic_MouseLeave(object sender, MouseEventArgs e)
        {
            Button senderBtn = (Button)sender;
            senderBtn.Opacity = 0.5;
        }
    }
}
