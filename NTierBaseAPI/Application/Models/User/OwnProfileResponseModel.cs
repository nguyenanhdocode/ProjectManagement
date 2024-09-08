using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.User
{
    public class OwnProfileResponseModel : ProfileResponseModel
    {
        public string Email { get; set; }
    }
}
