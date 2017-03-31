using System;
using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Domain;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Models;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Infrastructure.Mapper
{
    /// <summary>
    /// Represents AutoMapper configuration for extended reward points program models
    /// </summary>
    public class AutoMapperConfiguration : IMapperConfiguration
    {
        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns>Mapper configuration action</returns>
        public Action<IMapperConfigurationExpression> GetConfiguration()
        {
            return configuration =>
            {
                //common settings
                configuration.CreateMap<RewardPointsSettings, RewardPointsModel>()
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivatePointsImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.Title, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivationDelay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.IsEnabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.Message_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.Points_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                configuration.CreateMap<RewardPointsModel, RewardPointsSettings>();

                //settings on specific dates
                configuration.CreateMap<RewardPointsOnDateSettings, RewardPointsOnDateModel>()
                    .ForMember(dest => dest.AvailableCustomerRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomerRole, mo => mo.Ignore())
                    .ForMember(dest => dest.Store, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                configuration.CreateMap<RewardPointsOnDateModel, RewardPointsOnDateSettings>();
            };
        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }
}