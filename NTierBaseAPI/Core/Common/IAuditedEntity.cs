using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public interface IAuditedEntity
    {
        public string CreatedUserId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UpdatedUserId { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
