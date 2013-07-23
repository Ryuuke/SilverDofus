using System;
using System.IO;
using System.Text;

namespace SilverGame.Services
{
    static class Logs
    {
        private static readonly string[] LogsFolders =
        {
            Constant.ComFolder, Constant.GameFolder, Constant.ErrorsFolder
        };

        public static Object Lock = new object();

        private static readonly string FileName = string.Format("{0}.txt",DateTime.Now.ToString("yy-MM-dd"));

        public static void LoadLogs()
        {
            foreach (var folder in LogsFolders)
            {
                if(!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (!File.Exists(string.Format("{0}/{1}", folder, FileName)))
                    File.Create(string.Format("{0}/{1}", folder, FileName));
            }
        }

        public static void LogWritter(string folder, string text)
        {
            lock (Lock)
            {
                var sw = new StreamWriter(string.Format("{0}/{1}", folder, FileName), true, Encoding.Default);

                sw.Write(text + Environment.NewLine);

                sw.Close();
            }
        }
    }
}
