using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Core;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Domain;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Models;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services;
using Nop.Services.Configuration;
using localization = Nop.Services.Localization;
using Nop.Services.Customers;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Controllers
{
    [AdminAuthorize]
    public class ExtendedRewardPointsProgramController : BasePluginController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly localization.ILanguageService _languageService;
        private readonly localization.ILocalizationService _localizationService;
        private readonly localization.ILocalizedEntityService _localizedEntityService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IRewardPointsOnDateSettingsService _rewardPointsOnDateSettingsService;

        #endregion

        #region Ctor

        public ExtendedRewardPointsProgramController(ICustomerService customerService,
            localization.ILanguageService languageService,
            localization.ILocalizationService localizationService,
            localization.ILocalizedEntityService localizedEntityService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            IRewardPointsOnDateSettingsService rewardPointsOnDateSettingsService)
        {
            this._customerService = customerService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._rewardPointsOnDateSettingsService = rewardPointsOnDateSettingsService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected void PrepareModel(RewardPointsSettings settings, RewardPointsModel model, int storeScope, string title, string description)
        {
            //common settings
            model = settings.MapTo(model);

            //some of specific settings
            model.ActiveStoreScopeConfiguration = storeScope;
            model.ActivatePointsImmediately = settings.ActivationDelay <= 0;
            model.Title = title;
            model.Description = description;

            //localization. no multi-store support for localization yet
            AddLocales(_languageService, model.Locales, (locale, languageId) => 
                locale.Message = settings.GetLocalizedSetting(x => x.Message, languageId, 0, false, false));

            //overridable per store settings
            if (storeScope > 0)
            {
                model.IsEnabled_OverrideForStore = _settingService.SettingExists(settings, x => x.IsEnabled, storeScope);
                model.Points_OverrideForStore = _settingService.SettingExists(settings, x => x.Points, storeScope);
                model.ActivationDelay_OverrideForStore = _settingService.SettingExists(settings, x => x.ActivationDelay, storeScope);
                model.Message_OverrideForStore = _settingService.SettingExists(settings, x => x.Message, storeScope);
            }
        }

        [NonAction]
        protected void SaveSettings(RewardPointsSettings settings, RewardPointsModel model, int storeScope)
        {
            //common settings
            settings = model.MapTo(settings);

            //some of specific settings
            if (model.ActivatePointsImmediately)
                settings.ActivationDelay = 0;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(settings, x => x.IsEnabled, model.IsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.Points, model.Points_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.ActivationDelay, model.ActivationDelay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.ActivationDelayPeriodId, model.ActivationDelay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.Message, model.Message_OverrideForStore, storeScope, false);

            //localization. no multi-store support for localization yet
            foreach (var localized in model.Locales)
            {
                settings.SaveLocalizedSetting(x => x.Message, localized.LanguageId, localized.Message);
            }
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ExtendedRewardPointsProgramModel();

            //check for multistore
             model.IsMultistore = _storeService.GetAllStores().Count > 1;

            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);

            //prepare models
            PrepareModel(_settingService.LoadSetting<RewardPointsForBlogCommentsSettings>(storeScope), model.ForBlogComments, storeScope,
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForBlogComments"),
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForBlogComments.Hint"));

            PrepareModel(_settingService.LoadSetting<RewardPointsForFirstPurchaseSettings>(storeScope), model.ForFirstPurchase, storeScope,
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFirstPurchase"),
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFirstPurchase.Hint"));

            PrepareModel(_settingService.LoadSetting<RewardPointsForNewsCommentsSettings>(storeScope), model.ForNewsComments, storeScope,
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsComments"),
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsComments.Hint"));

            PrepareModel(_settingService.LoadSetting<RewardPointsForNewsletterSubscriptionsSettings>(storeScope), model.ForNewsletterSubscriptions, storeScope,
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsletterSubscriptions"),
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForNewsletterSubscriptions.Hint"));

            PrepareModel(_settingService.LoadSetting<RewardPointsForProductReviewsSettings>(storeScope), model.ForProductReviews, storeScope,
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForProductReviews"),
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForProductReviews.Hint"));

            PrepareModel(_settingService.LoadSetting<RewardPointsForRegistrationSettings>(storeScope), model.ForRegistration, storeScope,
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForRegistration"),
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForRegistration.Hint"));

            var rewardPointsForFastPurchaseSettings = _settingService.LoadSetting<RewardPointsForFastPurchaseSettings>(storeScope);
            PrepareModel(rewardPointsForFastPurchaseSettings, model.ForFastPurchase, storeScope,
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase"),
                _localizationService.GetResource("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase.Hint"));
            model.ForFastPurchase.Minutes = rewardPointsForFastPurchaseSettings.Minutes;
            model.ForFastPurchase.Minutes_OverrideForStore = storeScope > 0 && _settingService.SettingExists(rewardPointsForFastPurchaseSettings, x => x.Minutes, storeScope);

            return View("~/Plugins/Misc.ExtendedRewardPointsProgram/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        [AdminAntiForgery]
        public ActionResult Configure(ExtendedRewardPointsProgramModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);

            //save settings
            SaveSettings(_settingService.LoadSetting<RewardPointsForBlogCommentsSettings>(storeScope), model.ForBlogComments, storeScope);
            SaveSettings(_settingService.LoadSetting<RewardPointsForFirstPurchaseSettings>(storeScope), model.ForFirstPurchase, storeScope);
            SaveSettings(_settingService.LoadSetting<RewardPointsForNewsCommentsSettings>(storeScope), model.ForNewsComments, storeScope);
            SaveSettings(_settingService.LoadSetting<RewardPointsForNewsletterSubscriptionsSettings>(storeScope), model.ForNewsletterSubscriptions, storeScope);
            SaveSettings(_settingService.LoadSetting<RewardPointsForProductReviewsSettings>(storeScope), model.ForProductReviews, storeScope);
            SaveSettings(_settingService.LoadSetting<RewardPointsForRegistrationSettings>(storeScope), model.ForRegistration, storeScope);

            var rewardPointsForFastPurchaseSettings = _settingService.LoadSetting<RewardPointsForFastPurchaseSettings>(storeScope);
            SaveSettings(rewardPointsForFastPurchaseSettings, model.ForFastPurchase, storeScope);
            rewardPointsForFastPurchaseSettings.Minutes = model.ForFastPurchase.Minutes;
            _settingService.SaveSettingOverridablePerStore(rewardPointsForFastPurchaseSettings, x => x.Minutes, model.ForFastPurchase.Minutes_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #region Reward points on specific dates

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult RewardPointsOnDateList(DataSourceRequest command)
        {
            var allSettings = _rewardPointsOnDateSettingsService.GetAllRewardPointsOnDateSettings(pageIndex: command.Page - 1, pageSize: command.PageSize);
            var models = allSettings.Select(settings =>
            {
                //prepare common settings
                var model = settings.MapTo(new RewardPointsOnDateModel());

                //some of specific settings
                if (model.StoreId > 0)
                {
                    var store = _storeService.GetStoreById(model.StoreId);
                    model.Store = store != null ? store.Name : "Deleted";
                }
                else
                    model.Store = _localizationService.GetResource("Admin.Common.All");

                if (model.CustomerRoleId > 0)
                {
                    var role = _customerService.GetCustomerRoleById(model.CustomerRoleId);
                    model.CustomerRole = role != null ? role.Name : "Deleted";
                }
                else
                    model.CustomerRole = _localizationService.GetResource("Admin.Common.All");

                return model;
            }).ToList();

            return Json(new DataSourceResult
            {
                Data = models,
                Total = models.Count
            });
        }

        public ActionResult RewardPointsOnDateCreateOrUpdate(int id)
        {
            var model = new RewardPointsOnDateModel();
            var settings = _rewardPointsOnDateSettingsService.GetRewardPointsOnDateSettingsById(id);

            if (settings != null)
            {
                model = settings.MapTo(model);

                //localization
                AddLocales(_languageService, model.Locales, (locale, languageId) =>
                    locale.Message = localization.LocalizationExtensions.GetLocalized(settings, x => x.Message, languageId, false, false));
            }
            else
                AddLocales(_languageService, model.Locales);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            //customer roles
            model.AvailableCustomerRoles.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var role in _customerService.GetAllCustomerRoles(true))
                model.AvailableCustomerRoles.Add(new SelectListItem { Text = role.Name, Value = role.Id.ToString() });

            return View("~/Plugins/Misc.ExtendedRewardPointsProgram/Views/CreateOrUpdateRewardPointsOnDateSettings.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult RewardPointsOnDateCreateOrUpdate(string btnId, RewardPointsOnDateModel model)
        {
            var settings = _rewardPointsOnDateSettingsService.GetRewardPointsOnDateSettingsById(model.Id);
            if (settings != null)
            {
                //update existing settings
                settings = model.MapTo(settings);
                _rewardPointsOnDateSettingsService.UpdateRewardPointsOnDateSettings(settings);
            }
            else
            {
                //create new settings
                settings = model.MapTo(new RewardPointsOnDateSettings());
                _rewardPointsOnDateSettingsService.InsertRewardPointsOnDateSettings(settings);
            }

            //localization
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(settings, x => x.Message, localized.Message, localized.LanguageId);
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;

            return View("~/Plugins/Misc.ExtendedRewardPointsProgram/Views/CreateOrUpdateRewardPointsOnDateSettings.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult RewardPointsOnDateDelete(int id)
        {
            var settings = _rewardPointsOnDateSettingsService.GetRewardPointsOnDateSettingsById(id);
            if (settings != null)
                _rewardPointsOnDateSettingsService.DeleteRewardPointsOnDateSettings(settings);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}