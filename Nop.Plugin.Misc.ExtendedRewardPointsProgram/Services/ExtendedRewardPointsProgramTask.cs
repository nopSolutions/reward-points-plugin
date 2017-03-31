using System;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services
{
    /// <summary>
    /// Represents task for extended reward points program
    /// </summary>
    public partial class ExtendedRewardPointsProgramTask : ITask
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRewardPointsOnDateSettingsService _rewardPointsOnDateSettingsService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public ExtendedRewardPointsProgramTask(ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IRewardPointsOnDateSettingsService rewardPointsOnDateSettingsService,
            IRewardPointService rewardPointService,
            IStoreService storeService)
        {
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._rewardPointsOnDateSettingsService = rewardPointsOnDateSettingsService;
            this._rewardPointService = rewardPointService;
            this._storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            //find all settings of reward points that were to be awarded to the current date
            var allPastAwardedPointsSettings = _rewardPointsOnDateSettingsService.GetAllRewardPointsOnDateSettings(date: DateTime.UtcNow);

            //get not yet awarded
            var notAwarded = allPastAwardedPointsSettings.Where(settings => !settings.GetAttribute<bool>("CustomersAwardedOnDate")).ToList();

            //and now award it
            foreach (var settings in notAwarded)
            {
                //find users with appropriate customer roles
                var customerRoles = settings.CustomerRoleId > 0 ? new[] { settings.CustomerRoleId } : null;
                var customers = _customerService.GetAllCustomers(customerRoleIds: customerRoles);

                //get stores for which current awarding is actual
                var storeIds = settings.StoreId > 0 ? new[] { settings.StoreId }.ToList() : _storeService.GetAllStores().Select(store => store.Id).ToList();
                foreach (var storeId in storeIds)
                {
                    foreach (var customer in customers)
                    {
                        //get localized message for appropriate customer
                        var languageId = customer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId, storeId);
                        var message = settings.GetLocalized(setting => setting.Message, languageId);

                        //add reward points on specific date
                        _rewardPointService.AddRewardPointsHistoryEntry(customer, settings.Points, storeId, message);
                    }
                }

                //reward only once for each settings
                _genericAttributeService.SaveAttribute(settings, "CustomersAwardedOnDate", true);
            }
        }

        #endregion
    }
}
