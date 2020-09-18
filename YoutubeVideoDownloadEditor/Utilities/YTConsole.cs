using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeVideoDownloadEditor.Utilities
{
    public static class YTConsole
    {
        public delegate void MessagaArgs(string message);
        public static event MessagaArgs MessageReceived;

        public static void WriteLine(string text)
        {
            MessageReceived?.Invoke(text + '\n');
        }

        public static void Write(string text)
        {
            MessageReceived?.Invoke(text);
        }
    }
}
