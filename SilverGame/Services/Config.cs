﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SilverGame.Services
{
    static class Config
    {
        const string ConfigFile = "GameConfig.txt";

        private static Dictionary<string, string> values;

        public static void LoadConfig()
        {
            if (!File.Exists(ConfigFile))
                throw new Exception("unable to find the file : " + ConfigFile);

            values = new Dictionary<string, string>();

            var sr = new StreamReader(ConfigFile, Encoding.Default);

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
                Console.WriteLine("{0} inexistant ou illisible", ConfigFile);
            }

            sr.Close();
        }

        public static string get(string info)
        {
            return values[info];
        }
    }
}
