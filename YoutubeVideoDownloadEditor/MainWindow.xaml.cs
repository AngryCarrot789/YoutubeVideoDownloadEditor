using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YoutubeVideoDownloadEditor.Interfaces;
using YoutubeVideoDownloadEditor.ViewModels;

namespace YoutubeVideoDownloadEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMediaPlayer
    {
        public MainViewModel ViewModel
        {
            get => this.DataContext as MainViewModel;
            set => this.DataContext = value;
        }

        public TimeSpan Position => Player.Position;
        public Duration Duration => Player.NaturalDuration;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel(this);
        }

        public void Pause()
        {
            try
            {
                Player.Pause();
                IsPlaying = false;
            }
            catch { }
        }

        public void Play()
        {
            try
            {
                Player.Play();
                IsPlaying = true;
            }
            catch { }
        }

        public void Stop()
        {
            try
            {
                Player.Stop();
                IsPlaying = false;
            }
            catch { }
        }

        public void LoadUri(string path)
        {
            try
            {
                if (File.Exists(path))
                    Player.Source = new Uri(path);
            }
            catch { }
        }

        private bool IsPlaying { get; set; }
        private bool WasPlaying { get; set; }
        private bool SliderMouseDown { get; set; }

        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SliderMouseDown = true;
            WasPlaying = IsPlaying;
            ViewModel.Player.Pause();
        }

        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SliderMouseDown = false;
            if (WasPlaying)
                ViewModel.Player.AutoPlayPause();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderMouseDown)
                Player.Position = TimeSpan.FromSeconds(ViewModel.Player.PlayerPosition);
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            ViewModel.VideoLoaded();
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            ViewModel.VideoUnloaded();
        }
    }
}
