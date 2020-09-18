using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoutubeVideoDownloadEditor.Utilities;

namespace YoutubeVideoDownloadEditor.ViewModels
{
    public class VideoPreferencesViewModel : BaseViewModel
    {
        private string _urlPath;
        public string URLPath
        {
            get => _urlPath;
            set => RaisePropertyChanged(ref _urlPath, value);
        }

        private string _outputPath;
        public string OutputPath
        {
            get => _outputPath;
            set => RaisePropertyChanged(ref _outputPath, value);
        }

        private bool _isDownloadingOrExporting;
        public bool IsDownloadingOrExporting
        {
            get => _isDownloadingOrExporting;
            set => RaisePropertyChanged(ref _isDownloadingOrExporting, value);
        }

        private bool _fileDownloaded;
        public bool FileDownloaded
        {
            get => _fileDownloaded;
            set => RaisePropertyChanged(ref _fileDownloaded, value);
        }



        public string DownloadedFilePath { get; set; }

        public static string OUTPUT_DIR;

        public ICommand DownloadVideoCommand { get; }
        public ICommand BrowseOutputFileCommand { get; }

        public Action<string> LoadVideo { get; set; }

        public VideoPreferencesViewModel()
        {
            DownloadVideoCommand = new Command(DownloadVideo);
            BrowseOutputFileCommand = new Command(BrowseOutputPath);
            OUTPUT_DIR = Path.Combine(Path.GetTempPath(), "TEMP_YTDLE");
            if (!Directory.Exists(OUTPUT_DIR))
            {
                Directory.CreateDirectory(OUTPUT_DIR);
            }
        }

        public void DownloadVideo()
        {
            foreach (string file in Directory.GetFiles(OUTPUT_DIR))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }

            if (File.Exists(URLPath))
            {
                File.Copy(URLPath, Path.Combine(OUTPUT_DIR, "TEMPFILE" + Path.GetExtension(URLPath)));
                DownloadedFilePath = URLPath;
                FileDownloaded = true;
                LoadVideo?.Invoke(DownloadedFilePath);
            }
            else
            {
                Task.Run(() =>
                {
                    YTConsole.WriteLine($"Downloading '{URLPath}'");

                    IsDownloadingOrExporting = true;
                    string url = URLPath;
                    string youtubedlPath = Path.GetFullPath(@"..\..\apps\youtube-dl.exe");
                    ProcessStartInfo processInfo = new ProcessStartInfo(youtubedlPath, $"{url} --output {OUTPUT_DIR + @"\"}TEMPFILE");
                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;
                    processInfo.RedirectStandardError = true;
                    processInfo.RedirectStandardOutput = true;

                    Process process = Process.Start(processInfo);
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    process.Close();
                    IsDownloadingOrExporting = false;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (string file in Directory.GetFiles(OUTPUT_DIR))
                        {
                            if (file.Contains("TEMPFILE"))
                            {
                                DownloadedFilePath = file;
                                FileDownloaded = true;
                            }
                        }

                        LoadVideo?.Invoke(DownloadedFilePath);
                    });
                });
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            YTConsole.WriteLine(e.Data);
        }

        public void BrowseOutputPath()
        {
            if (File.Exists(DownloadedFilePath))
            {
                VistaSaveFileDialog sfd = new VistaSaveFileDialog();
                sfd.Title = "Select a location to save the exported file";
                sfd.Filter = "All files (*.*)|*.*";
                sfd.FileName = Path.GetFileName(DownloadedFilePath);

                if (sfd.ShowDialog() == true)
                {
                    OutputPath = sfd.FileName;
                }
            }
        }
    }
}
