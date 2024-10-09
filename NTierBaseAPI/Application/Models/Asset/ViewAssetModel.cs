using Application.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Asset
{
    public class ViewAssetModel : BaseReponseModel
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public double Size { get; set; }
        public string DisplayFileName { get; set; }

        public ViewProfileModel CreatedUser { get; set; }
    }
}
