using MusicApp.API;
using MusicApp.Core;
using MusicApp.MVVM.View;
using MusicApp.MVVM.ViewModel;
using NAudio.Utils;
using NAudio.Wave;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TagLib.Mpeg;

namespace MusicApp
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        Network network = new Network();
        List<Video> videos = new List<Video>();
        public List<Music> musicList = new();
        DispatcherTimer timer;
        MusicControls musicControl;
        Random random = new Random();




        double lastHeight = 720, lastWidth = 1280;
        internal MainViewModel Model { get; set; }

        public MainWindow()
        {
            PathWorking.CheckFolders();
            musicList = PathWorking.FetchMusics();
            Model = new MainViewModel(this); // MainViewModel oluşturulurken MainWindow örneği geçirilir
            DataContext = Model;
            musicControl = Model.MusicControl;


            InitializeComponent();



            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tick;
            timer.Start();


        }

        private void Tick(object sender, EventArgs e)
        {
            if (musicControl.waveOut.PlaybackState == PlaybackState.Playing && musicControl.isMusicSliding == false)
            {
                musicSlider.Value = musicControl.CalculatePercentage();
                elapsedTime.Content = musicControl.CalculateAndFormatElapsedTime();
            }
            else if(musicControl.waveOut.PlaybackState == PlaybackState.Stopped && musicControl.isUserPaused == false && musicControl.isMusicSliding == false)
            {
                int randomIndex = random.Next(0, musicList.Count);
                musicControl.PlayMusic(musicList[randomIndex].MusicPath, this.totalTime);
                ChangePlayButtonIcon("/MVVM/View/stopbutton.png", GetButtonDataGrid(randomIndex, Model.MusicView.dataGrid), this.playPauseMusicBtn);
                UpdateMainControlItems(musicList[randomIndex].Thumbnail.UriSource, musicList[randomIndex].Title);
                musicControl.currentMusicButton = GetButtonDataGrid(randomIndex, Model.MusicView.dataGrid);
                musicControl.currentMusicImage = this.playPauseMusicBtn;
                musicControl.currentMusicIndex = randomIndex + 1;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void musicControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Button buton = sender as Button;
            if (buton.Content is Image image)
            {
                image.Opacity = 1;
            }
        }
        private void musicControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Button buton = sender as Button;
            if (buton.Content is Image image)
            {
                image.Opacity = 0.5;
            }
        }
        private void SearchBox_MouseEnter(object sender, MouseEventArgs e)
        {
            searchBorder.Opacity = 1;
            searchButton.Opacity = 1;
        }

        private void SearchBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (SearchTextBox.Text == null || SearchTextBox.Text == "")
            {
                searchBorder.Opacity = 0.5;
                searchButton.Opacity = 0.5;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text == null || SearchTextBox.Text == "")
            {
                searchBorder.Opacity = 0.5;
                searchButton.Opacity = 0.5;
            }
            else
            {
                searchBorder.Opacity = 1;
                searchButton.Opacity = 1;

            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            if (Model.CurrentView == Model.SearchView)
            {
                // search panelinde ise bu işlemleri yap

                videos = await network.Search(SearchTextBox.Text, "10");

                SearchView view = (SearchView)contentController.Content;
                noContentLabel.Visibility = Visibility.Collapsed;
                view.dataGrid.ItemsSource = videos;

            }
            if (Model.CurrentView == Model.MusicView)
            {
                // musics panelinde ise bu işlemleri yap

            }
        }
        private async void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Model.CurrentView == Model.SearchView)
                {
                    // search panelinde ise bu işlemleri yap

                    videos = await network.Search(SearchTextBox.Text, "10");

                    SearchView view = (SearchView)contentController.Content;
                    noContentLabel.Visibility = Visibility.Collapsed;
                    view.dataGrid.ItemsSource = videos;

                }
                if (Model.CurrentView == Model.MusicView)
                {
                    // musics panelinde ise bu işlemleri yap

                }
            }
        }
        public void OnSearchViewEntered()
        {
            searchViewButton.Opacity = 1;
            MusicViewButton.Opacity=0.5;
            if (Model.SearchView.dataGrid.ItemsSource == null)
            {
                noContentLabel.Content = "Type something.";
                noContentLabel.Visibility = Visibility.Visible;
            }
            else
            {
                noContentLabel.Visibility = Visibility.Collapsed;
            }
        }
        public void OnMusicViewEntered()
        {
            MusicViewButton.Opacity=1;
            searchViewButton.Opacity = 0.5;
            if (musicList.Count == 0)
            {
                noContentLabel.Content = "You don't have any music.";
                noContentLabel.Visibility = Visibility.Visible;
            }
            else
            {
                noContentLabel.Visibility = Visibility.Collapsed;
            }
        }
        private void CloseAPP_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Opacity = 1;
        }

        private void CloseAPP_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Opacity = .5;
        }

        private void PlayPauseMainWindow_Click(object sender, RoutedEventArgs e)
        {
            if (musicControl.waveOut.PlaybackState == PlaybackState.Paused)
            {
                musicControl.ResumeMusic();
                ChangePlayButtonIcon("/MVVM/View/stopbutton.png", musicControl.currentMusicButton, this.playPauseMusicBtn);
            }
            else if (musicControl.waveOut.PlaybackState == PlaybackState.Playing)
            {
                musicControl.PauseMusic();
                ChangePlayButtonIcon("/MVVM/View/playbutton.png", musicControl.currentMusicButton, this.playPauseMusicBtn);
            }
            else if(musicControl.waveOut.PlaybackState == PlaybackState.Stopped && Model.CurrentView == Model.MusicView)
            {
                int randomIndex = random.Next(0, musicList.Count);
                musicControl.PlayMusic(musicList[randomIndex].MusicPath, this.totalTime);
                ChangePlayButtonIcon("/MVVM/View/stopbutton.png", GetButtonDataGrid(randomIndex, Model.MusicView.dataGrid), this.playPauseMusicBtn);
                UpdateMainControlItems(musicList[randomIndex].Thumbnail.UriSource, musicList[randomIndex].Title);
                musicControl.currentMusicButton = GetButtonDataGrid(randomIndex, Model.MusicView.dataGrid);
                musicControl.currentMusicImage = this.playPauseMusicBtn;
                musicControl.currentMusicIndex = randomIndex;
            }
        }
        private Button GetButtonDataGrid(int index, DataGrid grid)
        {
            if (grid != null)
            {
                // İlgili satır ve sütun numaralarını belirleyin
                int rowIndex = index; // 3. satır (sıfırdan başlayarak)
                int columnIndex = 0; // 1. sütun (sıfırdan başlayarak)

                // İlgili hücreyi bulun
                DataGridCell cell = GetCell(grid, rowIndex, columnIndex);

                if (cell != null)
                {
                    // Hücre içindeki Button'u bulun
                    Button playButton = FindVisualChild<Button>(cell);

                    if (playButton != null)
                    {
                        // playButton'u kullanın
                        return playButton;
                    }
                }
            }
            return null;
        }
        private void ChangePlayButtonIcon(string path, Button clickedButton, Image mainWindowIco)
        {
            Image playBtnIco = (Image)clickedButton.FindName("downloadImg");
            Uri imageUri = new Uri(path, UriKind.RelativeOrAbsolute);
            BitmapImage bitmap = new BitmapImage(imageUri);
            playBtnIco.Source = bitmap;
            mainWindowIco.Source = bitmap;
        }
        public void UpdateMainControlItems(Uri thumbnailURL,string musicName)
        {
            musicNameLabel.Content = musicName;
        }
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public static DataGridCell GetCell(DataGrid grid, int rowIndex, int columnIndex)
        {
            DataGridRow row = grid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    if (cell == null)
                    {
                        grid.ScrollIntoView(row, grid.Columns[columnIndex]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        private void PreviousMusic_Click(object sender, RoutedEventArgs e)
        {
            if (musicControl.currentMusicPath != null)
            {
                if (musicControl.waveOut.GetPositionTimeSpan() > new TimeSpan(0, 0, 3))
                {

                    Console.WriteLine(musicControl.waveOut.GetPositionTimeSpan());
                    musicControl.PlayMusic(musicControl.currentMusicPath, this.totalTime);

                }
                else
                {
                    if (musicControl.currentMusicIndex > 0)
                    {
                        musicControl.currentMusicIndex = musicControl.currentMusicIndex - 1;
                        Console.WriteLine(musicControl.currentMusicIndex);
                        musicControl.PlayMusic(musicList[musicControl.currentMusicIndex].MusicPath, this.totalTime);
                        UpdateMainControlItems(musicList[musicControl.currentMusicIndex].Thumbnail.UriSource, musicList[musicControl.currentMusicIndex].Title);
                        if (Model.CurrentView == Model.MusicView)
                        {
                            ChangePlayButtonIcon("/MVVM/View/playbutton.png", musicControl.currentMusicButton, this.playPauseMusicBtn);
                            ChangePlayButtonIcon("/MVVM/View/stopbutton.png", GetButtonDataGrid(musicControl.currentMusicIndex, Model.MusicView.dataGrid), this.playPauseMusicBtn);
                            musicControl.currentMusicButton = GetButtonDataGrid(musicControl.currentMusicIndex, Model.MusicView.dataGrid);
                            musicControl.currentMusicImage = this.playPauseMusicBtn;
                        }
                    }
                    else
                    {
                        musicControl.currentMusicIndex = musicList.Count - 1;
                        Console.WriteLine(musicControl.currentMusicIndex);
                        musicControl.PlayMusic(musicList[musicControl.currentMusicIndex].MusicPath,this.totalTime);
                        UpdateMainControlItems(musicList[musicControl.currentMusicIndex].Thumbnail.UriSource, musicList[musicControl.currentMusicIndex].Title);
                        if (Model.CurrentView == Model.MusicView)
                        {
                            ChangePlayButtonIcon("/MVVM/View/playbutton.png", musicControl.currentMusicButton, this.playPauseMusicBtn);
                            ChangePlayButtonIcon("/MVVM/View/stopbutton.png", GetButtonDataGrid(musicControl.currentMusicIndex, Model.MusicView.dataGrid), this.playPauseMusicBtn);
                            Console.WriteLine(musicControl.currentMusicIndex);
                            musicControl.currentMusicButton = GetButtonDataGrid(musicControl.currentMusicIndex, Model.MusicView.dataGrid);
                            musicControl.currentMusicImage = this.playPauseMusicBtn;
                        }
                    }
                }
                
            }
        }

        private void NextMusic_Click(object sender, RoutedEventArgs e)
        {
            if (musicControl.currentMusicPath != null)
            {
                if (musicControl.currentMusicIndex < musicList.Count - 1)
                {
                    musicControl.currentMusicIndex = musicControl.currentMusicIndex + 1;
                    Console.WriteLine(musicControl.currentMusicIndex);
                    musicControl.PlayMusic(musicList[musicControl.currentMusicIndex].MusicPath,this.totalTime);
                    UpdateMainControlItems(musicList[musicControl.currentMusicIndex].Thumbnail.UriSource, musicList[musicControl.currentMusicIndex].Title);
                    if (Model.CurrentView == Model.MusicView)
                    {
                        ChangePlayButtonIcon("/MVVM/View/playbutton.png", musicControl.currentMusicButton, this.playPauseMusicBtn);
                        ChangePlayButtonIcon("/MVVM/View/stopbutton.png", GetButtonDataGrid(musicControl.currentMusicIndex, Model.MusicView.dataGrid), this.playPauseMusicBtn);
                        musicControl.currentMusicButton = GetButtonDataGrid(musicControl.currentMusicIndex, Model.MusicView.dataGrid);
                        musicControl.currentMusicImage = this.playPauseMusicBtn;
                    }
                }
                else
                {
                    musicControl.currentMusicIndex = 0;
                    Console.WriteLine(musicControl.currentMusicIndex);
                    musicControl.PlayMusic(musicList[musicControl.currentMusicIndex].MusicPath, this.totalTime);
                    UpdateMainControlItems(musicList[musicControl.currentMusicIndex].Thumbnail.UriSource, musicList[musicControl.currentMusicIndex].Title);
                    if (Model.CurrentView == Model.MusicView )
                    {
                        ChangePlayButtonIcon("/MVVM/View/playbutton.png", musicControl.currentMusicButton, this.playPauseMusicBtn);
                        ChangePlayButtonIcon("/MVVM/View/stopbutton.png", GetButtonDataGrid(musicControl.currentMusicIndex, Model.MusicView.dataGrid), this.playPauseMusicBtn);
                        Console.WriteLine(musicControl.currentMusicIndex);
                        musicControl.currentMusicButton = GetButtonDataGrid(musicControl.currentMusicIndex, Model.MusicView.dataGrid);
                        musicControl.currentMusicImage = this.playPauseMusicBtn;
                    }
                }
            }
        }

        private void MuteMusic_Click(object sender, RoutedEventArgs e)
        {
            if(musicControl.waveOut != null)
            {
                if (musicControl.isMuted)
                {
                    volumeSlider.Value = musicControl.lastVolume * 100;
                    Console.WriteLine(musicControl.lastVolume * 100);
                    musicControl.waveOut.Volume = musicControl.lastVolume;
                    musicControl.isMuted = false;
                }
                else
                {
                    musicControl.lastVolume = Convert.ToSingle(volumeSlider.Value / 100);
                    volumeSlider.Value = 0;
                    musicControl.waveOut.Volume = 0;
                    musicControl.isMuted = true;
                }
                BitmapImage map = new BitmapImage();
                map.BeginInit();
                map.UriSource = new Uri(GetRequireVolumeIcon(volumeSlider.Value), UriKind.Relative);
                map.EndInit();
                volumeImage.Source = map;
            }
        }

        private void VolumeSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            musicControl.waveOut.Volume = (float)e.NewValue / 176;
            musicControl.isMuted = false;
            BitmapImage map = new BitmapImage();
            map.BeginInit();
            map.UriSource = new Uri(GetRequireVolumeIcon(volumeSlider.Value), UriKind.Relative);
            map.EndInit();
            volumeImage.Source = map;
        }
        private string GetRequireVolumeIcon(double value)
        {
            if(value < 0.99)
            {
                return VolumeIcons.Muted;
            }
            else if (value <= 25)
            {
                return VolumeIcons.LowVolume;
            }
            else if(value <= 50)
            {
                return VolumeIcons.MediumVolume;
            }
            else if(value <= 100)
            {
                return VolumeIcons.HighVolume;
            }
            return VolumeIcons.Muted;
        }

        private void MusicSlider_DragEnter(object sender, MouseButtonEventArgs e)
        {
            musicControl.isMusicSliding = true;
        }

        private void MusicSlider_DragLeave(object sender, MouseButtonEventArgs e)
        {
            if(musicControl.waveOut != null && musicControl.audioFile != null)
            {
                musicControl.isMusicSliding = false;
                double x = musicSlider.Value / 100;
                double sec = musicControl.audioFile.TotalTime.Minutes * 60 + musicControl.audioFile.TotalTime.Seconds;
                double previewSeconds = x * sec;
                musicControl.waveOut.Stop();
                musicControl.audioFile.CurrentTime = new TimeSpan(0,(int)(previewSeconds / 60), (int)(previewSeconds % 60));
                musicControl.waveOut.Play();
            }
        }

        private void MusicSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (musicControl.waveOut.PlaybackState == PlaybackState.Playing && musicControl.isMusicSliding)
            {
                elapsedTime.Content = musicControl.CalculatePreviewElapsedTime(musicSlider.Value);
            }
        }
       

        private void MinimizeAPP_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }



}

