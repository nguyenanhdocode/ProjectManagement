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
        public virtual Project Project { get; set; }
        public virtual Asset Asset { get; set; }
    }
}
