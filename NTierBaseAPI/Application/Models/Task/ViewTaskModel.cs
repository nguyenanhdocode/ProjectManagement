using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class ViewTaskModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public AppTaskStatus Status { get; set; }
    }
}
