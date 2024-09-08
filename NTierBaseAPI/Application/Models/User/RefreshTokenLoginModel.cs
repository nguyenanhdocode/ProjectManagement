using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.User
{
    public class RefreshTokenLoginModel
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenLoginResponseModel
    {
        public string AccessToken { get; set; }
    }
}
