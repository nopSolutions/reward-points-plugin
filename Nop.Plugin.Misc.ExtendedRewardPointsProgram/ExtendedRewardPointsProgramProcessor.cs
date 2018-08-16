using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Data;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram
{
    /// <summary>
    /// Represents extended reward points program processor
    /// </summary>
    public class ExtendedRewardPointsProgramProcessor : BasePlugin, IMiscPlugin
    {
        #region Constants

        private const string EXTENDED_REWARD_POINTS_PROGRAM_TASK_TYPE = "Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services.ExtendedRewardPointsProgramTask, Nop.Plugin.Misc.ExtendedRewardPointsProgram";

        #endregion

        #region Fields

        private readonly IBlogService _blogService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly INewsService _newsService;
        private readonly IProductService _productService;
        private readonly IRewardPointsOnDateSettingsService _rewardPointsOnDateSettingsService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly ExtendedRewardPointsProgramObjectContext _objectContext;
        private readonly IWebHelper _webHelper;
        
        #endregion

        #region Ctor

        public ExtendedRewardPointsProgramProcessor(IBlogService blogService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INewsService newsService,
            IProductService productService,
            IRewardPointsOnDateSettingsService rewardPointsOnDateSettingsService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            ExtendedRewardPointsProgramObjectContext objectContext,
            IWebHelper webHelper)
        {
            this._blogService = blogService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._newsService = newsService;
            this._productService = productService;
            this._rewardPointsOnDateSettingsService = rewardPointsOnDateSettingsService;
            this._scheduleTaskService = scheduleTaskService;
            this._settingService = settingService;
            this._objectContext = objectContext;
            this._webHelper = webHelper;
        }

        #endregion
        
        #region Methods
        
        /// <summary>
        /// Gets a route for plugin configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ExtendedRewardPointsProgram";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Misc.ExtendedRewardPointsProgram.Controllers" }, { "area", null } };
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ExtendedRewardPointsProgram/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //database objects
            _objectContext.Install();

            //settings
            _settingService.SaveSetting(new RewardPointsForBlogCommentsSettings
            {
                Message = "Earned promotion for the comment to blog post {0}"
            });

            _settingService.SaveSetting(new RewardPointsForFastPurchaseSettings
            {
                Message = "Earned promotion for the fast purchase",
                Minutes = 15
            });

            _settingService.SaveSetting(new RewardPointsForFirstPurchaseSettings
            {
                Message = "Earned promotion for the first purchase"
            });

            _settingService.SaveSetting(new RewardPointsForNewsCommentsSettings
            {
                Message = "Earned promotion for the comment to news {0}"
            });

            _settingService.SaveSetting(new RewardPointsForNewsletterSubscriptionsSettings
            {
                Message = "Earned promotion for the newsletter subscription"
            });

            _settingService.SaveSetting(new RewardPointsForProductReviewsSettings
            {
                Message = "Earned promotion for the review to product {0}"
            });

            _settingService.SaveSetting(new RewardPointsForRegistrationSettings
            {
                Message = "Earned promotion for the registration"
            });

            //task for awarding on specific dates
            if (_scheduleTaskService.GetTaskByType(EXTENDED_REWARD_POINTS_PROGRAM_TASK_TYPE) == null)
            {
                _scheduleTaskService.InsertTask(new ScheduleTask
                {
                    Enabled = true,
                    Name = "Extended reward points program task",
                    Seconds = 3600,
                    Type = EXTENDED_REWARD_POINTS_PROGRAM_TASK_TYPE
                });
            }

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram", "Extended reward points settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivatePointsImmediately", "Activate points immediately");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivatePointsImmediately.Hint", "Activates bonus points immediately after their calculation");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivationDelay", "Reward points activation");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivationDelay.Hint", "Specify how many days (hours) must elapse before earned points become active.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.IsEnabled", "Enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.IsEnabled.Hint", "Check to enable reward points program.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Message", "Message");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Message.Hint", "Enter the message for reward points history.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Points", "Points");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Points.Hint", "Specify number of awarded points");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForBlogComments", "Reward points for blog post comments");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForBlogComments.Hint", "Points are awarded after a customer left a comment to a certain blog post and it has been approved by store owner.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase", "Reward points for the fast purchase");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase.Fields.Minutes", "Minutes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase.Fields.Minutes.Hint", "Specify the time interval in minutes during which the user must complete a purchase.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase.Hint", "Points are awarded after an order get paid in a certain period after a customer add product to cart.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFirstPurchase", "Reward points for the first purchase");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFirstPurchase.Hint", "Points are awarded after a customer made a first purchase in online store.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsComments", "Reward points for news comments");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsComments.Hint", "Points are awarded after a customer left a comment to a certain news item and it has been approved by store owner.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsletterSubscriptions", "Reward points for newsletter subscriptions");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsletterSubscriptions.Hint", "Points are awarded after a customer subscribed to a newsletter.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForProductReviews", "Reward points for product reviews");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForProductReviews.Hint", "Points are awarded after a customer left a review to a product and it has been approved by store owner.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForRegistration", "Reward points for the registration");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForRegistration.Hint", "Points are awarded after a customer passed a registration.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate", "Reward points on specific date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.AddNew", "Add new reward points on specific date settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Edit", "Edit reward points on specific date settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.AwardingDate", "Date of awarding");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.AwardingDate.Hint", "Specify date and time of awarding in UTC");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.CustomerRole", "Customer role");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.CustomerRole.Hint", "Select customer role for which the settings will be available.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.Store", "Store");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.Store.Hint", "Option to limit this settings to a certain store.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Hint", "It’s possible to award points on specific dates, for examples holidays. Customers will receive a message notification about that. That functionality can be limited by customer roles and stores.");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //generic attributes
            var onDateSettings = _rewardPointsOnDateSettingsService.GetAllRewardPointsOnDateSettings().ToList();
            onDateSettings.ForEach(setting => _genericAttributeService.SaveAttribute(setting, "CustomersAwardedOnDate", string.Empty));

            _blogService.GetAllComments().ToList().ForEach(comment =>
                _genericAttributeService.SaveAttribute(comment, "CustomerAwardedForBlogComment", string.Empty, comment.StoreId));

            _newsService.GetAllComments().ToList().ForEach(comment =>
                _genericAttributeService.SaveAttribute(comment, "CustomerAwardedForNewsComment", string.Empty, comment.StoreId));

            _productService.GetAllProductReviews(0, null).ToList().ForEach(review =>
                _genericAttributeService.SaveAttribute(review, "CustomerAwardedForProductReview", string.Empty, review.StoreId));

            _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions().ToList().ForEach(subscription =>
                _genericAttributeService.SaveAttribute(subscription, "CustomerAwardedForSubscription", string.Empty, subscription.StoreId));

            _customerService.GetAllCustomers().ToList().ForEach(customer =>
                _genericAttributeService.DeleteAttributes(_genericAttributeService.GetAttributesForEntity(customer.Id, "Customer")
                    .Where(attribute => attribute.Key.Equals("PurchaseStartTime")).ToList()));

            //localized properties
            var localizedSettings = new[] 
            {
                _settingService.GetSetting("RewardPointsForBlogCommentsSettings.Message"),
                _settingService.GetSetting("RewardPointsForFastPurchaseSettings.Message"),
                _settingService.GetSetting("RewardPointsForFirstPurchaseSettings.Message"),
                _settingService.GetSetting("RewardPointsForNewsCommentsSettings.Message"),
                _settingService.GetSetting("RewardPointsForNewsletterSubscriptionsSettings.Message"),
                _settingService.GetSetting("RewardPointsForProductReviewsSettings.Message")
            }.Where(setting => setting != null).ToList();

            foreach (var language in _languageService.GetAllLanguages(true))
            {
                localizedSettings.ForEach(setting => _localizedEntityService.SaveLocalizedValue(setting, x => x.Value, string.Empty, language.Id));
                onDateSettings.ForEach(setting => _localizedEntityService.SaveLocalizedValue(setting, x => x.Message, string.Empty, language.Id));
            }

            //database objects
            _objectContext.Uninstall();

            //settings
            _settingService.DeleteSetting<RewardPointsForBlogCommentsSettings>();
            _settingService.DeleteSetting<RewardPointsForFastPurchaseSettings>();
            _settingService.DeleteSetting<RewardPointsForFirstPurchaseSettings>();
            _settingService.DeleteSetting<RewardPointsForNewsCommentsSettings>();
            _settingService.DeleteSetting<RewardPointsForNewsletterSubscriptionsSettings>();
            _settingService.DeleteSetting<RewardPointsForProductReviewsSettings>();
            _settingService.DeleteSetting<RewardPointsForRegistrationSettings>();

            //scheduled task
            var task = _scheduleTaskService.GetTaskByType(EXTENDED_REWARD_POINTS_PROGRAM_TASK_TYPE);
            if (task != null)
                _scheduleTaskService.DeleteTask(task);

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivatePointsImmediately");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivatePointsImmediately.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivationDelay");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivationDelay.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.IsEnabled");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.IsEnabled.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Message");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Message.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Points");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Points.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForBlogComments");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForBlogComments.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFirstPurchase");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFirstPurchase.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase.Fields.Minutes");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase.Fields.Minutes.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsComments");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsComments.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsletterSubscriptions");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsletterSubscriptions.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForProductReviews");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForProductReviews.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForRegistration");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.ForRegistration.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Edit");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.AwardingDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.AwardingDate.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.CustomerRole");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.CustomerRole.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.Store");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.Store.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Hint");

            base.Uninstall();
        }

        #endregion
    }
}
