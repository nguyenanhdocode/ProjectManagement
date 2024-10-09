using Application.Common;
using Application.Models.Asset;
using AutoMapper;
using Core.Entities;
using Microsoft.Extensions.Options;

namespace Application.MappingProfiles
{
    public class AssetProfile : Profile
    {
        public AssetProfile()
        {
            CreateMap<Asset, ViewAssetModel>()
                .ForMember(p => p.Url, m => m.MapFrom<AbsolutePathResolver>());
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

    public class AbsolutePathResolver : IValueResolver<Asset, ViewAssetModel, string>
    {
        private StaticConfiguration _staticConfiguration;
        private AppConfiguration _appConfiguration;

        public AbsolutePathResolver(IOptions<StaticConfiguration> staticConfiguration
            , IOptions<AppConfiguration> appConfiguration)
        {
            _staticConfiguration = staticConfiguration.Value;
            _appConfiguration = appConfiguration.Value;
        }

        public string Resolve(Asset source, ViewAssetModel destination, string destMember, ResolutionContext context)
        {

            return string.Format("{0}{1}/{2}", _appConfiguration.Domain
                , _staticConfiguration.StaticUrl, source.FileName);
        }
    }
}
