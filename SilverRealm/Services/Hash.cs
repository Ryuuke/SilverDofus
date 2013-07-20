using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverRealm.Services
{
    static class Hash
    {
        private static Random rand = new Random();

        // tableau de caractère pour le hashage du mots de passe

        public static char[] hash = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
                't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};

        // genère un string de lenght lengueur qui servira à créer une clé pour identifier chaque compte

        public static string RandomString(int lenght)
        {
            var str = string.Empty;

            for (var i = 1; i <= lenght; i++)
            {
                var randomInt = rand.Next(0, hash.Length);
                str += hash[randomInt];
            }

            return str;
        }

        public static string Encrypt(string password, string key)
        {
            var _Crypted = "1";

            for (var i = 0; i < password.Length; i++)
            {
                var PPass = password[i];
                var PKey = key[i];
                var APass = (int)PPass / 16;
                var AKey = (int)PPass % 16;
                var ANB = (APass + (int)PKey) % hash.Length;
                var ANB2 = (AKey + (int)PKey) % hash.Length;

                _Crypted += hash[ANB];
                _Crypted += hash[ANB2];
            }

            return _Crypted;
        }
    }
}
