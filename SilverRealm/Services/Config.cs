using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SilverRealm.Services
{
    static class Config
    {
        const string ConfigFile = Constant.ConfigFile;

        private static Dictionary<string, string> _values;

        public static void LoadConfig()
        {
            if(!File.Exists(ConfigFile))
                throw new Exception("Unable to find the file : "+ConfigFile);

            _values = new Dictionary<string, string>();

            var sr = new StreamReader(ConfigFile, Encoding.Default);
            
            try
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (line != null && (line.Trim() != string.Empty && line.Trim().StartsWith("//") == false))
                    {
                        var infos = line.Split('=');
                        _values.Add(infos[0].Trim(), infos[1].Trim());
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("{0} absent or unreadable", ConfigFile);
            }

            sr.Close();
        }

        public static string Get(string info)
        {
            return _values[info];
        }
    }
}
