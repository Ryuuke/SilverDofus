using System;

namespace SilverGame.Services
{
    static class Algorithm
    {
        public static string GenerateRandomName()
        {
            const string consonant = "aeiouy";
            const string vowel = "bcdfghjklmnpqrstvwxz";
            
            var name = string.Empty;

            var rand = new Random();
            
            var nameLength = rand.Next(3, 10);

            for (var i = 0; i < nameLength; i++)
            {
                name += i%2 == 0 ? consonant[rand.Next(consonant.Length - 1)] : vowel[rand.Next(vowel.Length - 1)];
            }

            return name;
        }
    }
}
