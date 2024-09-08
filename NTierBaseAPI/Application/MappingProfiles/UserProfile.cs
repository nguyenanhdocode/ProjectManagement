using Application.Models.User;
using AutoMapper;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserModel, AppUser>();

            CreateMap<AppUser, OwnProfileResponseModel>()
                .ForMember(p => p.AvatarUrl, m => m.MapFrom(u => (u.Avatar != null) ? u.Avatar.Path : null));
            
            CreateMap<AppUser, ProfileResponseModel>()
                .ForMember(p => p.AvatarUrl, m => m.MapFrom(u => (u.Avatar != null) ? u.Avatar.Path : null));
        }
    }
}
