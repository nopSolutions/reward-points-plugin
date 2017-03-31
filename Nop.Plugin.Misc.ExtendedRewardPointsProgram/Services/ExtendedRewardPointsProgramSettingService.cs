using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Configuration;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services
{
    /// <summary>
    /// Represents override settings service (just for using custom setting extensions)
    /// </summary>
    public partial class ExtendedRewardPointsProgramSettingService : SettingService
    {
        #region Ctor

        public ExtendedRewardPointsProgramSettingService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<Setting> settingRepository) : base(cacheManager, eventPublisher, settingRepository) 
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>true -setting exists; false - does not exist</returns>
        public override bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0)
        {
            var key = settings.GetSettingKey(keySelector);
            var setting = GetSettingByKey<string>(key, storeId: storeId);

            return setting != null;
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public override void SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0, bool clearCache = true)
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", keySelector));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", keySelector));

            var key = settings.GetSettingKey(keySelector);

            //Duck typing is not supported in C#. That's why we're using dynamic type
            dynamic value = propInfo.GetValue(settings, null);
            SetSetting(key, value != null ? value : string.Empty, storeId, clearCache);
        }

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        public override void DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0)
        {
            var key = settings.GetSettingKey(keySelector).Trim().ToLowerInvariant();

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ? allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;

            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                DeleteSetting(setting);
            }
        }

        #endregion
    }
}
