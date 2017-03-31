using Nop.Data.Mapping;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Domain;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Data
{
    /// <summary>
    /// Represents map configuration for RewardPointsOnDateSettings class
    /// </summary>
    public class RewardPointsOnDateSettingsMap : NopEntityTypeConfiguration<RewardPointsOnDateSettings>
    {
        public RewardPointsOnDateSettingsMap()
        {
            this.ToTable("RewardPointsOnDateSettings");
            this.HasKey(points => points.Id);
        }
    }
}