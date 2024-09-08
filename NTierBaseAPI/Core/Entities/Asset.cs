using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Asset : BaseEntity
    {
        public string Path { get; set; }
        public string AssetId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; }
        public string? CreatedUserId { get; set; }
        public AppUser CreatedUser { get; set; }
        public AppUser Avatar { get; set; }
        public ICollection<ProjectAsset> ProjectAssets { get; set; }
        public ICollection<TaskAsset> TaskAssets { get; set; }
    }
}
