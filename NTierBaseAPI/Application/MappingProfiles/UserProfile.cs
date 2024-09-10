using Application.Common;
using Application.Models.Asset;
using Application.Models.User;
using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

            CreateMap<AppUser, OwnProfileResponseModel>();

            CreateMap<AppUser, ProfileResponseModel>()
                .ForMember(p => p.AvatarUrl, m => m.MapFrom<AvatarUrlResolver>())
                .Include<AppUser, OwnProfileResponseModel>();
        }
    }

    public class AvatarUrlResolver : IValueResolver<AppUser, ProfileResponseModel, string>
    {
        private StaticConfiguration _staticConfiguration;
        public AvatarUrlResolver(IOptions<StaticConfiguration> staticConfiguration)
        {
            _staticConfiguration = staticConfiguration.Value;
        }

        public string Resolve(AppUser source, ProfileResponseModel destination, string destMember, ResolutionContext context)
        {
            if (source.Avatar != null)
                return string.Format("{0}/{1}", _staticConfiguration.StaticUrl, source.Avatar.FileName);

            return string.Empty;
        }
    }
}
