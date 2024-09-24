using Application.Models.Project;
using Application.Services;
using AutoMapper;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<CreateProjectModel, Project>();
            CreateMap<Project, ViewProjectOverviewModel>();
            CreateMap<Project, ViewProjectModel>()
                .Include<Project, ViewProjectOverviewModel>()
                .ForMember(p => p.IsManager, m => m.MapFrom<IsManagerResolver>());
        }

        public class IsManagerResolver : IValueResolver<Project, ViewProjectModel, bool>
        {
            private IClaimService _claimService;

            public IsManagerResolver(IClaimService claimService)
            {
                _claimService = claimService;
            }

            public bool Resolve(Project source, ViewProjectModel destination, bool destMember, ResolutionContext context)
            {
                return _claimService.GetUserId().Equals(source.ManagerId);
            }
        }
    }
}
