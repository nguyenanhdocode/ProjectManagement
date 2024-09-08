using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.User
{
    public static class UserValidatorConfiguration
    {
        public const int MaximumFirstNameLength = 35;

        public const int MaximumLastNameLength = 35;

        public const int MinimumPasswordLength = 8;

        public const int MaximumPasswordLength = 128;

        public const int MaximumEmailLength = 256;

        public const string EmailRegex = "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$";

        public const String PasswordRegex = "^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$";
    }
}
