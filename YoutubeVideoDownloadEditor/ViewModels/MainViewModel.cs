using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoutubeVideoDownloadEditor.Interfaces;
using YoutubeVideoDownloadEditor.Utilities;

namespace YoutubeVideoDownloadEditor.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private double _rangeSliderMinimum;
        public double RangeSliderMinimum
        {
            get => _rangeSliderMinimum;
            set => RaisePropertyChanged(ref _rangeSliderMinimum, value);
        }

        private double _rangeSliderMaximum;
        public double RangeSliderMaximum
        {
            get => _rangeSliderMaximum;
            set => RaisePropertyChanged(ref _rangeSliderMaximum, value);
        }

        private double _rangeLower;
        public double RangeLower
        {
            get => _rangeLower;
            set
            {
                RaisePropertyChanged(ref _rangeLower, value);
            }
        }

        private double _rangeUpper;
        public double RangeUpper
        {
            get => _rangeUpper;
            set => RaisePropertyChanged(ref _rangeUpper, value);
        }

        public ICommand SetRangeLowerToPositionCommand { get; }
        public ICommand SetRangeUpperToPositionCommand { get; }
        public ICommand ExportVideoCommand { get; }

        public MediaPlayerViewModel Player { get; set; }

        public VideoPreferencesViewModel VideoPreferences { get; set; }

        public MainViewModel(IMediaPlayer player)
        {
            Player = new MediaPlayerViewModel(player);
            VideoPreferences = new VideoPreferencesViewModel();
            VideoPreferences.LoadVideo = LoadVideo;

            SetRangeLowerToPositionCommand = new Command(SetRangeLowerToPosition);
            SetRangeUpperToPositionCommand = new Command(SetRangeUpperToPosition);
            ExportVideoCommand = new Command(ExportVideo);

            //Player.LoadVideo(@"C:\Users\kettl\Desktop\animalbeep.mp4");
        }

        private void LoadVideo(string path)
        {
            Player.LoadVideo(path);
        }

        public void ExportVideo()
        {
            if (File.Exists(VideoPreferences.DownloadedFilePath))
            {
                string fileName = Path.GetFileName(VideoPreferences.DownloadedFilePath);
                string outputFileName = Path.GetFileName(VideoPreferences.OutputPath);
                if (File.Exists(Path.Combine(Path.GetPathRoot(outputFileName), fileName)))
                {
                    MessageBox.Show("File already exists. chose another location.");
                }
                else
                {
                    Task.Run(() =>
                    {
                        VideoPreferences.IsDownloadingOrExporting = true;
                        string url = VideoPreferences.URLPath;
                        string ffmpegPath = Path.GetFullPath(@"..\..\apps\ffmpeg.exe");
                        if (RangeLower >= 0 && (RangeUpper >= 0 && RangeUpper > RangeLower))
                        {
                            string startTimeStr = TimeSpan.FromSeconds(RangeLower).ToString(@"hh\:mm\:ss\.ffff");
                            double endTime = RangeUpper - RangeLower;
                            string endTimeStr = TimeSpan.FromSeconds(endTime).ToString(@"hh\:mm\:ss\.ffff");
                            ProcessStartInfo processInfo = new ProcessStartInfo(ffmpegPath, $"-ss {startTimeStr} -t {endTimeStr} -i {VideoPreferences.DownloadedFilePath} {VideoPreferences.OutputPath}");

                            processInfo.CreateNoWindow = true;
                            processInfo.UseShellExecute = false;
                            processInfo.RedirectStandardError = true;
                            processInfo.RedirectStandardOutput = true;

                            Process process = Process.Start(processInfo);
                            process.OutputDataReceived += Process_OutputDataReceived;
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();
                            process.WaitForExit();
                            process.Close();
                        }
                        VideoPreferences.IsDownloadingOrExporting = false;
                    });
                }
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        public void VideoLoaded()
        {
            RangeSliderMaximum = Player.Player.Duration.TimeSpan.TotalSeconds;
            RangeLower = 0;
            RangeUpper = RangeSliderMaximum;
            Player.VideoLoaded();
        }

        public void VideoUnloaded()
        {
            Player.VideoUnloaded();
        }

        public void SetRangeLowerToPosition()
        {
            RangeLower = Player.PlayerPosition;
        }

        public void SetRangeUpperToPosition()
        {
            RangeUpper = Player.PlayerPosition;
        }
    }
}
