using Application.Common;
using Application.Models.Asset;
using AutoMapper;
using Core.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class AssetProfile : Profile
    {
        public AssetProfile()
        {
            CreateMap<Asset, ViewAssetModel>()
                .ForMember(p => p.RelativeUrl, m => m.MapFrom<AssetToViewAssetModelResolver>());
            CreateMap<Asset, CreateAssetResponseModel>();
        }
    }

    public class AssetToViewAssetModelResolver : IValueResolver<Asset, ViewAssetModel, string>
    {
        private StaticConfiguration _staticConfiguration;
        public AssetToViewAssetModelResolver(IOptions<StaticConfiguration> staticConfiguration)
        {
            _staticConfiguration = staticConfiguration.Value;
        }

        public string Resolve(Asset source, ViewAssetModel destination, string destMember, ResolutionContext context)
        {
            if (source == null)
                return string.Empty;

            return string.Format("{0}/{1}", _staticConfiguration.StaticUrl, source.FileName);
        }
    }
}
