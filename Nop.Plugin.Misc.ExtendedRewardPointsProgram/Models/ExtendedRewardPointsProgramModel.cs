using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Models
{
    public class ExtendedRewardPointsProgramModel : BaseNopModel
    {
        public ExtendedRewardPointsProgramModel()
        {
            ForBlogComments = new RewardPointsModel();
            ForFirstPurchase = new RewardPointsModel();
            ForNewsComments = new RewardPointsModel();
            ForNewsletterSubscriptions = new RewardPointsModel();
            ForProductReviews = new RewardPointsModel();
            ForRegistration = new RewardPointsModel();
            ForFastPurchase = new RewardPointsForFastPurchaseModel();
        }

        public bool IsMultistore { get; set; }

        public RewardPointsModel ForBlogComments { get; set; }

        public RewardPointsModel ForFirstPurchase { get; set; }

        public RewardPointsModel ForNewsComments { get; set; }

        public RewardPointsModel ForNewsletterSubscriptions { get; set; }

        public RewardPointsModel ForProductReviews { get; set; }

        public RewardPointsModel ForRegistration { get; set; }

        public RewardPointsForFastPurchaseModel ForFastPurchase { get; set; }
    }

    public class RewardPointsModel : BaseNopModel, ILocalizedModel<LocalizedModel>
    {
        public RewardPointsModel()
        {
            Locales = new List<LocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.Fields.IsEnabled")]
        public bool IsEnabled { get; set; }
        public bool IsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Points")]
        public int Points { get; set; }
        public bool Points_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivatePointsImmediately")]
        public bool ActivatePointsImmediately { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.Fields.ActivationDelay")]
        public int ActivationDelay { get; set; }
        public bool ActivationDelay_OverrideForStore { get; set; }
        public int ActivationDelayPeriodId { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Message")]
        public string Message { get; set; }
        public bool Message_OverrideForStore { get; set; }

        public IList<LocalizedModel> Locales { get; set; }
    }

    public class LocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Message")]
        public string Message { get; set; }
    }

    public class RewardPointsOnDateModel : BaseNopEntityModel, ILocalizedModel<LocalizedModel>
    {
        public RewardPointsOnDateModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailableCustomerRoles = new List<SelectListItem>();
            Locales = new List<LocalizedModel>();
        }

        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Points")]
        public int Points { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.AwardingDate")]
        [UIHint("DateTime")]
        public DateTime AwardingDateUtc { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.Fields.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.CustomerRole")]
        public int CustomerRoleId { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
        public string CustomerRole { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.RewardPointsOnDate.Fields.Store")]
        public int StoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public string Store { get; set; }

        public IList<LocalizedModel> Locales { get; set; }
    }

    public class RewardPointsForFastPurchaseModel : RewardPointsModel
    {
        [NopResourceDisplayName("Plugins.Misc.ExtendedRewardPointsProgram.ForFastPurchase.Fields.Minutes")]
        public int Minutes { get; set; }
        public bool Minutes_OverrideForStore { get; set; }
    }
}