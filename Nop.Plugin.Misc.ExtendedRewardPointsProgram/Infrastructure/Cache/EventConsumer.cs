using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Infrastructure.Cache
{
    /// <summary>
    /// Represents extended reward points program event consumer
    /// </summary>
    public partial class EventConsumer :
        IConsumer<BlogCommentApprovedEvent>,
        IConsumer<NewsCommentApprovedEvent>,
        IConsumer<ProductReviewApprovedEvent>,
        IConsumer<CustomerRegisteredEvent>,
        IConsumer<EmailSubscribedEvent>,
        IConsumer<OrderPaidEvent>,
        IConsumer<EntityInsertedEvent<ShoppingCartItem>>
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderService _orderService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly RewardPointsForBlogCommentsSettings _rewardPointsSettingsForBlogComments;
        private readonly RewardPointsForFastPurchaseSettings _rewardPointsSettingsForFastPurchase;
        private readonly RewardPointsForFirstPurchaseSettings _rewardPointsSettingsForFirstPurchase;
        private readonly RewardPointsForNewsCommentsSettings _rewardPointsSettingsForNewsComments;
        private readonly RewardPointsForNewsletterSubscriptionsSettings _rewardPointsSettingsForNewsletterSubscriptions;
        private readonly RewardPointsForProductReviewsSettings _rewardPointsSettingsForProductReviews;
        private readonly RewardPointsForRegistrationSettings _rewardPointsForRegistrationSettings;

        #endregion

        #region Ctor

        public EventConsumer(ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IOrderService orderService,
            IRewardPointService rewardPointService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            RewardPointsForBlogCommentsSettings rewardPointsSettingsForBlogComments,
            RewardPointsForFastPurchaseSettings rewardPointsForFastPurchaseSettings,
            RewardPointsForFirstPurchaseSettings rewardPointsSettingsForFirstPurchase,
            RewardPointsForNewsCommentsSettings rewardPointsSettingsForNewsComments,
            RewardPointsForNewsletterSubscriptionsSettings rewardPointsSettingsForNewsletterSubscriptions,
            RewardPointsForProductReviewsSettings rewardPointsSettingsForProductReviews,
            RewardPointsForRegistrationSettings rewardPointsForRegistrationSettings)
        {
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._orderService = orderService;
            this._rewardPointService = rewardPointService;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._rewardPointsSettingsForBlogComments = rewardPointsSettingsForBlogComments;
            this._rewardPointsSettingsForFastPurchase = rewardPointsForFastPurchaseSettings;
            this._rewardPointsSettingsForFirstPurchase = rewardPointsSettingsForFirstPurchase;
            this._rewardPointsSettingsForNewsComments = rewardPointsSettingsForNewsComments;
            this._rewardPointsSettingsForNewsletterSubscriptions = rewardPointsSettingsForNewsletterSubscriptions;
            this._rewardPointsSettingsForProductReviews = rewardPointsSettingsForProductReviews;
            this._rewardPointsForRegistrationSettings = rewardPointsForRegistrationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get reward points activation date
        /// </summary>
        /// <param name="settings">Reward points settings</param>
        /// <returns>Date and time or null for the instant activation</returns>
        protected DateTime? GetRewardPointsActivationDate(RewardPointsSettings settings)
        {
            if (settings.ActivationDelay <= 0)
                return null;

            var delayPeriod = (RewardPointsActivatingDelayPeriod)settings.ActivationDelayPeriodId;
            var delayInHours = delayPeriod.ToHours(settings.ActivationDelay);

            return DateTime.UtcNow.AddHours(delayInHours);
        }

        #endregion

        #region Methods

        #region Blog comments

        public void HandleEvent(BlogCommentApprovedEvent blogCommentEvent)
        {
            if (!_rewardPointsSettingsForBlogComments.IsEnabled)
                return;

            if (blogCommentEvent.BlogComment.Customer == null)
                return;

            //reward user only for the first approving
            if (_genericAttributeService.GetAttribute<bool>(blogCommentEvent.BlogComment, "CustomerAwardedForBlogComment", blogCommentEvent.BlogComment.StoreId))
                return;

            //check whether delay is set
            var activationDate = GetRewardPointsActivationDate(_rewardPointsSettingsForBlogComments);

            //get message for the current customer
            var languageId = _genericAttributeService.GetAttribute<int>(blogCommentEvent.BlogComment.Customer, NopCustomerDefaults.LanguageIdAttribute, blogCommentEvent.BlogComment.StoreId);
            var message = _rewardPointsSettingsForBlogComments.GetLocalizedSetting(settings => settings.Message, languageId, blogCommentEvent.BlogComment.StoreId);

            //add reward points for approved blog post comment
            _rewardPointService.AddRewardPointsHistoryEntry(blogCommentEvent.BlogComment.Customer, _rewardPointsSettingsForBlogComments.Points,
                blogCommentEvent.BlogComment.StoreId, string.Format(message, blogCommentEvent.BlogComment.BlogPost.Title), activatingDate: activationDate);

            //mark that customer was already awarded for the comment
            _genericAttributeService.SaveAttribute(blogCommentEvent.BlogComment, "CustomerAwardedForBlogComment", true, blogCommentEvent.BlogComment.StoreId);
        }

        #endregion

        #region News comments

        public void HandleEvent(NewsCommentApprovedEvent newsCommentEvent)
        {
            if (!_rewardPointsSettingsForNewsComments.IsEnabled)
                return;

            if (newsCommentEvent.NewsComment.Customer == null)
                return;

            //reward user only for the first approving
            if (_genericAttributeService.GetAttribute<bool>(newsCommentEvent.NewsComment, "CustomerAwardedForNewsComment", newsCommentEvent.NewsComment.StoreId))
                return;

            //check whether delay is set
            var activationDate = GetRewardPointsActivationDate(_rewardPointsSettingsForNewsComments);

            //get message for the current customer
            var languageId = _genericAttributeService.GetAttribute<int>(newsCommentEvent.NewsComment.Customer, NopCustomerDefaults.LanguageIdAttribute, newsCommentEvent.NewsComment.StoreId);
            var message = _rewardPointsSettingsForNewsComments.GetLocalizedSetting(settings => settings.Message, languageId, newsCommentEvent.NewsComment.StoreId);

            //add reward points for approved news comment
            _rewardPointService.AddRewardPointsHistoryEntry(newsCommentEvent.NewsComment.Customer, _rewardPointsSettingsForNewsComments.Points,
                newsCommentEvent.NewsComment.StoreId, string.Format(message, newsCommentEvent.NewsComment.NewsItem.Title), activatingDate: activationDate);

            //mark that customer was already awarded for the comment
            _genericAttributeService.SaveAttribute(newsCommentEvent.NewsComment, "CustomerAwardedForNewsComment", true, newsCommentEvent.NewsComment.StoreId);
        }

        #endregion

        #region Product reviews

        public void HandleEvent(ProductReviewApprovedEvent productReviewEvent)
        {
            if (!_rewardPointsSettingsForProductReviews.IsEnabled)
                return;

            if (productReviewEvent.ProductReview.Customer == null)
                return;

            //reward user only for the first approving
            if (_genericAttributeService.GetAttribute<bool>(productReviewEvent.ProductReview, "CustomerAwardedForProductReview", productReviewEvent.ProductReview.StoreId))
                return;

            //check whether delay is set
            var activationDate = GetRewardPointsActivationDate(_rewardPointsSettingsForProductReviews);

            //get message for the current customer
            var languageId = _genericAttributeService.GetAttribute<int>(productReviewEvent.ProductReview.Customer, NopCustomerDefaults.LanguageIdAttribute, productReviewEvent.ProductReview.StoreId);
            var message = _rewardPointsSettingsForProductReviews.GetLocalizedSetting(settings => settings.Message, languageId, productReviewEvent.ProductReview.StoreId);
            var productName = _localizationService.GetLocalized(productReviewEvent.ProductReview.Product, product => product.Name, languageId);

            //add reward points for approved product review
            _rewardPointService.AddRewardPointsHistoryEntry(productReviewEvent.ProductReview.Customer, _rewardPointsSettingsForProductReviews.Points,
                productReviewEvent.ProductReview.StoreId, string.Format(message, productName), activatingDate: activationDate);

            //mark that customer was already awarded for the review
            _genericAttributeService.SaveAttribute(productReviewEvent.ProductReview, "CustomerAwardedForProductReview", true, productReviewEvent.ProductReview.StoreId);
        }

        #endregion

        #region Registration

        public void HandleEvent(CustomerRegisteredEvent registeredEvent)
        {
            if (!_rewardPointsForRegistrationSettings.IsEnabled)
                return;

            if (registeredEvent.Customer == null)
                return;

            //check whether delay is set
            var activationDate = GetRewardPointsActivationDate(_rewardPointsForRegistrationSettings);

            //get message for the current customer
            var languageId = _genericAttributeService.GetAttribute<int>(registeredEvent.Customer, NopCustomerDefaults.LanguageIdAttribute, _storeContext.CurrentStore.Id);
            var message = _rewardPointsForRegistrationSettings.GetLocalizedSetting(settings => settings.Message, languageId, _storeContext.CurrentStore.Id);

            //add reward points for approved product review
            _rewardPointService.AddRewardPointsHistoryEntry(registeredEvent.Customer, _rewardPointsForRegistrationSettings.Points,
                _storeContext.CurrentStore.Id, message, activatingDate: activationDate);
        }

        #endregion

        #region Email subscriptions

        public void HandleEvent(EmailSubscribedEvent subscribedEvent)
        {
            if (!_rewardPointsSettingsForNewsletterSubscriptions.IsEnabled)
                return;

            //find customer
            var customer = _customerService.GetCustomerByEmail(subscribedEvent.Subscription.Email);
            if (customer == null)
                return;

            //reward user only for the first subscription
            if (_genericAttributeService.GetAttribute<bool>(subscribedEvent.Subscription, "CustomerAwardedForSubscription", subscribedEvent.Subscription.StoreId))
                return;

            //check whether delay is set
            var activationDate = GetRewardPointsActivationDate(_rewardPointsSettingsForNewsletterSubscriptions);

            //get message for the current customer
            var languageId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.LanguageIdAttribute, subscribedEvent.Subscription.StoreId);
            var message = _rewardPointsSettingsForNewsletterSubscriptions.GetLocalizedSetting(settings => settings.Message, languageId, subscribedEvent.Subscription.StoreId);

            //add reward points for newsletter subscription
            _rewardPointService.AddRewardPointsHistoryEntry(customer, _rewardPointsSettingsForNewsletterSubscriptions.Points,
                subscribedEvent.Subscription.StoreId, message, activatingDate: activationDate);

            _genericAttributeService.SaveAttribute(subscribedEvent.Subscription, "CustomerAwardedForSubscription", true, subscribedEvent.Subscription.StoreId);
        }

        #endregion

        #region Fast purchase

        public void HandleEvent(EntityInsertedEvent<ShoppingCartItem> insertedShoppingCartItem)
        {
            if (!_rewardPointsSettingsForFastPurchase.IsEnabled)
                return;

            if (insertedShoppingCartItem.Entity.Customer == null)
                return;

            if (insertedShoppingCartItem.Entity.Customer.ShoppingCartItems.Count(item => item.StoreId == insertedShoppingCartItem.Entity.StoreId) > 1)
                return;

            //record time of the first adding shopping cart item
            _genericAttributeService.SaveAttribute<DateTime?>(insertedShoppingCartItem.Entity.Customer, "PurchaseStartTime", DateTime.UtcNow, insertedShoppingCartItem.Entity.StoreId);
        }

        #endregion

        #region First purchase

        public void HandleEvent(OrderPaidEvent orderPaidEvent)
        {
            if (orderPaidEvent.Order.Customer == null)
                return;

            //reward points for the first purchase
            if (_rewardPointsSettingsForFirstPurchase.IsEnabled)
            {
                //check whether this order is the first
                var paidOrders = _orderService.SearchOrders(customerId: orderPaidEvent.Order.Customer.Id, psIds: new[] { (int)PaymentStatus.Paid }.ToList());
                if (paidOrders.TotalCount > 1)
                    return;

                //check whether delay is set
                var activationDate = GetRewardPointsActivationDate(_rewardPointsSettingsForFirstPurchase);

                //get message for the current customer
                var message = _rewardPointsSettingsForFirstPurchase.GetLocalizedSetting(settings => settings.Message,
                    orderPaidEvent.Order.CustomerLanguageId, orderPaidEvent.Order.StoreId);

                //add reward points for the first purchase
                _rewardPointService.AddRewardPointsHistoryEntry(orderPaidEvent.Order.Customer, _rewardPointsSettingsForFirstPurchase.Points,
                    orderPaidEvent.Order.StoreId, message, activatingDate: activationDate);
            }

            //reward points for the fast purchase
            if (_rewardPointsSettingsForFastPurchase.IsEnabled)
            {
                //get start time of purchase
                var purchaseStartTime = _genericAttributeService.GetAttribute<DateTime?>(orderPaidEvent.Order.Customer, "PurchaseStartTime", orderPaidEvent.Order.StoreId);
                if (!purchaseStartTime.HasValue)
                    return;

                //clear start time
                _genericAttributeService.SaveAttribute<DateTime?>(orderPaidEvent.Order.Customer, "PurchaseStartTime", null, orderPaidEvent.Order.StoreId);

                //compare the time of purchase with the set time span
                if (DateTime.UtcNow.Subtract(purchaseStartTime.Value) > TimeSpan.FromMinutes(_rewardPointsSettingsForFastPurchase.Minutes ?? 0))
                    return;

                //check whether delay is set
                var activationDate = GetRewardPointsActivationDate(_rewardPointsSettingsForFastPurchase);

                //get message for the current customer
                var message = _rewardPointsSettingsForFastPurchase.GetLocalizedSetting(settings => settings.Message,
                    orderPaidEvent.Order.CustomerLanguageId, orderPaidEvent.Order.StoreId);

                //add reward points for the first purchase
                _rewardPointService.AddRewardPointsHistoryEntry(orderPaidEvent.Order.Customer, _rewardPointsSettingsForFastPurchase.Points,
                    orderPaidEvent.Order.StoreId, message, activatingDate: activationDate);
            }
        }

        #endregion

        #endregion
    }
}
