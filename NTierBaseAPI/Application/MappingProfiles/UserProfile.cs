using Application.Common;
using Application.Models.Asset;
using Application.Models.User;
using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Http;
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

            CreateMap<AppUser, ViewProfileModel>()
                .ForMember(p => p.AvatarUrl, m => m.MapFrom<AvatarUrlResolver>())
                .Include<AppUser, OwnProfileResponseModel>();
        }
    }

    public class AvatarUrlResolver : IValueResolver<AppUser, ViewProfileModel, string>
    {
        private StaticConfiguration _staticConfiguration;
        private AppConfiguration _appConfiguration;

        public AvatarUrlResolver(IOptions<StaticConfiguration> staticConfiguration
            , IOptions<AppConfiguration> appConfiguration)
        {
            _staticConfiguration = staticConfiguration.Value;
            _appConfiguration = appConfiguration.Value;
        }

        public string Resolve(AppUser source, ViewProfileModel destination, string destMember, ResolutionContext context)
        {
            if (source.Avatar != null)
            {
                return string.Format("{0}{1}/{2}", _appConfiguration.Domain
                    , _staticConfiguration.StaticUrl, source.Avatar.FileName);
            }

            return string.Empty;
        }
    }
}
