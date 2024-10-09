using Application.Models.Task;
using AutoMapper;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<CreateTaskModel, AppTask>();
            CreateMap<CreateSubtaskModelBody, AppTask>();
            CreateMap<AppTask, ViewTaskOverviewResponseModel>()
                .ForMember(p => p.RemainHours, m => m.MapFrom(u => (u.EndDate - DateTime.UtcNow).TotalHours));
            CreateMap<AppTask, ViewTaskModel>()
                .Include<AppTask, ViewTaskOverviewResponseModel>();
        }
    }
}
