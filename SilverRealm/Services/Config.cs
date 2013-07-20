using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SilverRealm.Services
{
    static class Config
    {
        const string configFile = "RealmConfig.txt";

        private static Dictionary<string, string> values;

        public static void loadConfig()
        {
            if(!File.Exists(configFile))
                throw new Exception("unable to find the file : "+configFile);

            values = new Dictionary<string, string>();

            StreamReader sr = new StreamReader(configFile, Encoding.Default);
            
            try
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.Trim() != string.Empty && line.Trim().StartsWith("//") == false)
                    {
                        var infos = line.Split('=');
                        values.Add(infos[0].Trim(), infos[1].Trim());
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine(string.Format("{0} inexistant ou illisible", configFile));
            }

            sr.Close();
        }

        public static string get(string info)
        {
            return values[info];
        }
    }
}
