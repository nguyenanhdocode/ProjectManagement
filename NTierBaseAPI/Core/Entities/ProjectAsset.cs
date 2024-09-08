using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProjectAsset
    {
        public string ProjectId { get; set; }
        public Guid AssetId { get; set; }
        public Project Project { get; set; }
        public Asset Asset { get; set; }
    }
}
