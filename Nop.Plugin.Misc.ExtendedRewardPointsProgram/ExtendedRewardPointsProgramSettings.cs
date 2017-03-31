using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram
{
    /// <summary>
    /// Represents extended reward points settings
    /// </summary>
    public class RewardPointsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether reward points program is enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a number of awarded points
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets a delay before activation points
        /// </summary>
        public int ActivationDelay { get; set; }

        /// <summary>
        /// Gets or sets the period (int value of RewardPointsActivatingDelayPeriod enum) of activation delay
        /// </summary>
        public int ActivationDelayPeriodId { get; set; }

        /// <summary>
        /// Gets or sets a message for reward points history
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents reward point settings for blog comments
    /// </summary>
    public class RewardPointsForBlogCommentsSettings : RewardPointsSettings { }

    /// <summary>
    /// Represents reward point settings for the first purchase
    /// </summary>
    public class RewardPointsForFirstPurchaseSettings : RewardPointsSettings { }

    /// <summary>
    /// Represents reward point settings for news comments
    /// </summary>
    public class RewardPointsForNewsCommentsSettings : RewardPointsSettings { }

    /// <summary>
    /// Represents reward point settings for newsletter subscriptions
    /// </summary>
    public class RewardPointsForNewsletterSubscriptionsSettings : RewardPointsSettings { }

    /// <summary>
    /// Represents reward point settings for product reviews
    /// </summary>
    public class RewardPointsForProductReviewsSettings : RewardPointsSettings { }

    /// <summary>
    /// Represents reward point settings for the registration
    /// </summary>
    public class RewardPointsForRegistrationSettings : RewardPointsSettings { }

    /// <summary>
    /// Represents reward point settings for the fast purchase
    /// </summary>
    public class RewardPointsForFastPurchaseSettings : RewardPointsSettings
    {
        /// <summary>
        /// Gets or sets the time span in minutes during which the user must complete a purchase
        /// </summary>
        public int Minutes { get; set; }
    }
}
