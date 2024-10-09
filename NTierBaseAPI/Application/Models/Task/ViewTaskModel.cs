using Application.Common;
using Application.Models.User;
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
        public string ProjectId { get; set; }
        public string PreviousTaskId { get; set; }
        public string Note {  get; set; }
        public DateTime DoneDate { get; set; }

        public ViewProfileModel AssignedToUser { get; set; }
        public ViewProfileModel CreatedUser { get; set; }
        public ViewTaskModel PreviousTask { get; set; }
    }
}
