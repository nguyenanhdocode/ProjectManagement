﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.User
{
    public class ResetPasswordModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}