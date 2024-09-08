using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TaskAsset
    {
        public string TaskId { get; set; }
        public Guid AssetId { get; set; }
        public AppTask Task { get; set; }
        public Asset Asset { get; set; }
    }
}
