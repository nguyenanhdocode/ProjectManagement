using Application.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Project
{
    public class ViewProjectOverviewModel : ViewProjectModel
    {
        public int RemainHours { get; set; }
        public ViewProfileModel Manager { get; set; }
    }
}
