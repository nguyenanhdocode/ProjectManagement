using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class RandomPassword
    {
        private static readonly string _lowerCases = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string _upperCases = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string _numbers = "0123456789";
        private static readonly string _specialChars = "@$!%*#?&";

        public static string GeneratePassword(int numLowerCase = 2, int numUpperCase = 2
            , int numNumbers = 2, int numSpecialChars = 2)
        {
            var password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < numLowerCase; i++)
            {
                int randIndex = random.Next(0, _lowerCases.Length);
                password.Append(_lowerCases[randIndex]);
            }

            for (int i = 0; i < numUpperCase; i++)
            {
                int randIndex = random.Next(0, _upperCases.Length);
                password.Append(_upperCases[randIndex]);
            }

            for (int i = 0; i < numNumbers; i++)
            {
                int randIndex = random.Next(0, _numbers.Length);
                password.Append(_numbers[randIndex]);
            }

            for (int i = 0; i < numSpecialChars; i++)
            {
                int randIndex = random.Next(0, _specialChars.Length);
                password.Append(_specialChars[randIndex]);
            }

            return new string(password.ToString().ToCharArray()
                .OrderBy(s => (random.Next(2) % 2) == 0).ToArray());
        }
    }
}
