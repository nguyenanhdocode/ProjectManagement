using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Asset
{
    public class AssetReponseModel : BaseReponseModel
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
