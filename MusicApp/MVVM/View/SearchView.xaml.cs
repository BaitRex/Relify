using MusicApp.Core;
using MusicApp;
using MusicApp.API;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using MusicApp.MVVM.ViewModel;
using NAudio.Wave;
using System.Net;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;
using AngleSharp.Common;

namespace MusicApp.MVVM.View
{
    /// <summary>
    /// SearchView.xaml etkileşim mantığı
    /// </summary>
    public partial class SearchView : UserControl
    {
        MainViewModel viewModel;
        Network network;
        internal SearchView(MainViewModel viewmod)
        {
            viewModel = viewmod;
            network = new Network();
            InitializeComponent();
        }

        private void Download_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Opacity = 1f;
        }

        private void Download_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Opacity = 0.5f;

        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            DataGridCell cell = FindVisualParent<DataGridCell>(clickedButton);
            if (cell != null)
            {
                // Veri satırını bulma
                var video = (cell.DataContext as Video);
                if (video != null)
                {
                    if (video.DownloadIcon == DownloadIcons.DownloadIcon) 
                    {
                        video.DownloadIcon = DownloadIcons.LoadingIcon;
                        Image img = (Image)clickedButton.FindName("downloadImg");
                        if (img != null)
                        {
                            string path = @"loading-cropped.gif"; // Yeni resmin yolunu ayarlayın

                            // Yeni resmi yükleme
                            BitmapImage map = new BitmapImage();
                            map.BeginInit();
                            map.UriSource = new Uri(path, UriKind.Relative);
                            map.EndInit();

                            // Butonun içindeki resmi değiştirme
                            ImageBehavior.SetAnimatedSource(img, map);
                        }

                        // Butonun bulunduğu hücreyi bulma
                        if (cell != null)
                        {
                            // Veri satırını bulma
                            if (video != null)
                            {
                                // Veri satırındaki özelliklere erişim
                                string thumbnail = video.Thumbnail;
                                string title = video.Title;
                                string videoID = video.VideoID;

                                await network.GetDownloadURL(videoID, Directory.GetCurrentDirectory() + @"\Musics\",thumbnail,title);

                                // Butonun içindeki resmi değiştirme
                                if (img != null)
                                {
                                    string path = @"download-finished.png"; // Yeni resmin yolunu ayarlayın

                                    // Yeni resmi yükleme
                                    BitmapImage map = new BitmapImage();
                                    map.BeginInit();
                                    map.UriSource = new Uri(path, UriKind.Relative);
                                    map.EndInit();

                                    // Butonun içindeki resmi değiştirme
                                    ImageBehavior.SetAnimatedSource(img, map);
                                }
                                viewModel.MainWindow.musicList = PathWorking.FetchMusics();
                                viewModel.MusicView.dataGrid.ItemsSource = viewModel.MainWindow.musicList;
                                foreach (Music m in viewModel.MainWindow.musicList)
                                {
                                    Console.WriteLine(m.ID);
                                    Console.WriteLine(m.Title);
                                }
                            }
                        }
                    }
                }
            }


            

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

        

        
    }
}
