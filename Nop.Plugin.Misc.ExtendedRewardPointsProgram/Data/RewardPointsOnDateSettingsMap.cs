using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Domain;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Data
{
    /// <summary>
    /// Represents map configuration for RewardPointsOnDateSettings class 
    /// </summary>
    public partial class RewardPointsOnDateSettingsMap : NopEntityTypeConfiguration<RewardPointsOnDateSettings>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<RewardPointsOnDateSettings> builder)
        {
            builder.ToTable(nameof(RewardPointsOnDateSettings));
            builder.HasKey(points => points.Id);
        }

        #endregion
    }
}