using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class UniqueIdHelper
    {
        private static readonly string BASE64_CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";

        public static string GenerateId(int length)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                builder.Append(BASE64_CHARS[random.Next(0, BASE64_CHARS.Length)]);
            }

            return builder.ToString();
        }
    }
}
