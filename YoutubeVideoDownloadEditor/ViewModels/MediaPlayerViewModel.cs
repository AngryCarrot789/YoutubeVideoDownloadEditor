using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using YoutubeVideoDownloadEditor.Interfaces;
using YoutubeVideoDownloadEditor.Utilities;

namespace YoutubeVideoDownloadEditor.ViewModels
{
    public class MediaPlayerViewModel : BaseViewModel
    {
        private double _playerSliderMinimum;
        public double PlayerSliderMinimum
        {
            get => _playerSliderMinimum;
            set => RaisePropertyChanged(ref _playerSliderMinimum, value);
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set => RaisePropertyChanged(ref _isPlaying, value);
        }

        private double _playerSliderMaximum;
        public double PlayerSliderMaximum
        {
            get => _playerSliderMaximum;
            set => RaisePropertyChanged(ref _playerSliderMaximum, value);
        }

        private double _playerPosition;
        public double PlayerPosition
        {
            get => _playerPosition;
            set => RaisePropertyChanged(ref _playerPosition, value);
        }

        private double _playerDuration;
        public double PlayerDuration
        {
            get => _playerDuration;
            set => RaisePropertyChanged(ref _playerDuration, value);
        }

        public ICommand AutoPlayPauseCommand { get; }
        public ICommand StopCommand { get; }

        public IMediaPlayer Player { get; }

        private DispatcherTimer PositionTimer { get; set; }

        public MediaPlayerViewModel(IMediaPlayer player)
        {
            Player = player;
            PositionTimer = new DispatcherTimer();
            AutoPlayPauseCommand = new Command(AutoPlayPause);
            StopCommand = new Command(Stop);
            PositionTimer.Interval = TimeSpan.FromMilliseconds(100);
            PositionTimer.Tick += PositionTimer_Tick;
        }

        private void PositionTimer_Tick(object sender, EventArgs e)
        {
            PlayerPosition = Player.Position.TotalSeconds;
        }

        public void LoadVideo(string path)
        {
            Player.LoadUri(path);
            Stop();
        }

        public void AutoPlayPause()
        {
            if (IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        public void Play()
        {
            IsPlaying = true;
            Player.Play();
            PositionTimer.Start();
        }

        public void Pause()
        {
            IsPlaying = false;
            Player.Pause();
            PositionTimer.Stop();
        }

        public void Stop()
        {
            Player.Stop();
            PositionTimer.Stop();
            ResetPosition();
        }

        public void ResetPosition()
        {
            PlayerPosition = 0;
        }

        public void VideoLoaded()
        {
            PlayerSliderMaximum = Player.Duration.TimeSpan.TotalSeconds;
            if (Player.Duration.HasTimeSpan)
                PlayerDuration = Player.Duration.TimeSpan.TotalSeconds;
        }

        public void VideoUnloaded()
        {
            PlayerSliderMaximum = 1;
            PositionTimer.Stop();
        }
    }
}
