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
        public string AssetId { get; set; }
        public string RelativePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; }
        public string? CreatedUserId { get; set; }
        public string FileName { get; set; }
        public double Size { get; set; }
        public AppUser CreatedUser { get; set; }
        public AppUser AvatarUser { get; set; }
        public ICollection<ProjectAsset> ProjectAssets { get; set; }
        public ICollection<TaskAsset> TaskAssets { get; set; }
    }
}
