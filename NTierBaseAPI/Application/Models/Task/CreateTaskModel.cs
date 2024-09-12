using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Task
{
    public class CreateTaskModel
    {
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Note { get; set; }
        public string AssignedToUserId { get; set; }
        public string ProjectId { get; set; }
        public string? PreviousTaskId { get; set; }
    }

    public class CreateTaskResponseModel
    {
        public string Id { get; set; }
    }
}
