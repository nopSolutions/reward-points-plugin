using System;
using Nop.Core;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Domain;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services
{
    /// <summary>
    /// Represents service interface for the reward points on specific date settings
    /// </summary>
    public interface IRewardPointsOnDateSettingsService
    {
        /// <summary>
        /// Gets all reward points on specific dates settings
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="roleId">Customer role identifier; pass 0 to load all records</param>
        /// <param name="date">Date and time in UTC; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reward points on specific dates settings</returns>
        IPagedList<RewardPointsOnDateSettings> GetAllRewardPointsOnDateSettings(int storeId = 0, int roleId = 0,
            DateTime? date = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets reward points on specific date settings by identifier
        /// </summary>
        /// <param name="settingsId">Settings identifier</param>
        /// <returns>Reward points on specific date settings</returns>
        RewardPointsOnDateSettings GetRewardPointsOnDateSettingsById(int settingsId);

        /// <summary>
        /// Inserts reward points on specific date settings
        /// </summary>
        /// <param name="settings">Reward points on specific date settings</param>
        void InsertRewardPointsOnDateSettings(RewardPointsOnDateSettings settings);

        /// <summary>
        /// Updates reward points on specific date settings
        /// </summary>
        /// <param name="settings">Reward points on specific date settings</param>
        void UpdateRewardPointsOnDateSettings(RewardPointsOnDateSettings settings);

        /// <summary>
        /// Deletes reward points on specific date settings
        /// </summary>
        /// <param name="settings">Reward points on specific date settings</param>
        void DeleteRewardPointsOnDateSettings(RewardPointsOnDateSettings settings);
    }
}
