using MusicApp.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicAppDesign.MVVM.Class;
using MusicApp.Core;

namespace MusicApp.MVVM.ViewModel
{
	class MainViewModel : ObservableObject
    {
        public RelayCommand SearchViewCommand { get; set; }
        public RelayCommand MusicViewCommand { get; set; }

        public SearchView SearchView {  get; set; }
        public MusicView MusicView {  get; set; }
        public MusicControls MusicControl { get; set; }
        public MainWindow MainWindow { get; set; }
        public object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
           
        }

        public MainViewModel(MainWindow mainWindowRef)
        {
            SearchView = new SearchView(this);
            MusicControl = new MusicControls();
            MusicView = new MusicView(this);
            MainWindow = mainWindowRef;

            CurrentView = SearchView;
            SearchViewCommand = new RelayCommand(o => 
            {
                CurrentView = SearchView;
                mainWindowRef.OnSearchViewEntered();
            });

            MusicViewCommand = new RelayCommand(o =>
            {
                CurrentView = MusicView;
                mainWindowRef.OnMusicViewEntered();
            });
        }
    }
}
