using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SilverRealm.Services
{
    static class Config
    {
        private static Dictionary<string, string> _values;

        public static bool LoadConfig()
        {
            if (!File.Exists(Constant.ConfigFile))
                SilverConsole.WriteLine("Error : Unable to find the file : " + Constant.ConfigFile, ConsoleColor.Red);

            _values = new Dictionary<string, string>();

            try
            {
                using (var sr = new StreamReader(Constant.ConfigFile, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        if (line == null || (line.Trim() == string.Empty || line.Trim().StartsWith("//")))
                            continue;

                        var infos = line.Split('=');
                        _values.Add(infos[0].Trim(), infos[1].Trim());
                    }
                }

                return true;
            }
            catch (Exception)
            {
                SilverConsole.WriteLine(string.Format("Error : {0} unreadable", Constant.ConfigFile), ConsoleColor.Red);

                return false;
            }
        }

        public static string Get(string info)
        {
            try
            {
                return _values[info];
            }
            catch (Exception e)
            {
                SilverConsole.WriteLine(string.Format("Unable to find {0} in {1}", info, Constant.ConfigFile));
                return null;
            }
        }
    }
}
