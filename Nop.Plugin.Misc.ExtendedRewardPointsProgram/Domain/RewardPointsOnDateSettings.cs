using System;
using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Domain
{
    /// <summary>
    /// Represents a settings for reward points on specific date
    /// </summary>
    public class RewardPointsOnDateSettings : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets a number of awarded points
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets a date and time of awarding in UTC
        /// </summary>
        public DateTime AwardingDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a message for reward points history
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the store identifier (0 for all stores)
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier (0 for all roles)
        /// </summary>
        public int CustomerRoleId { get; set; }
    }
}