using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SilverRealm.Services
{
    static class Config
    {
        private static Dictionary<string, string> _values;

        public static void LoadConfig()
        {
            if(!File.Exists(Constant.ConfigFile))
                throw new Exception("Unable to find the file : "+ Constant.ConfigFile);

            _values = new Dictionary<string, string>();

            var sr = new StreamReader(Constant.ConfigFile, Encoding.Default);
            
            try
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
            catch (Exception)
            {
                Console.WriteLine("{0} absent or unreadable", Constant.ConfigFile);
            }

            sr.Close();
        }

        public static string Get(string info)
        {
            return _values[info];
        }
    }
}
