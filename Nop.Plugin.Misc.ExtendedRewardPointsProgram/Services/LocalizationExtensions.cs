using System;
using System.Linq.Expressions;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services
{
    /// <summary>
    /// Represents custom localization extensions (with using custom setting extensions)
    /// </summary>
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Get localized property of setting
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static string GetLocalizedSetting<T>(this T settings, Expression<Func<T, string>> keySelector, int languageId, int storeId,
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true) where T : ISettings, new()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            var key = settings.GetSettingKey(keySelector);

            //we do not support localized settings per store (overridden store settings)
            var setting = settingService.GetSetting(key, storeId: storeId, loadSharedValueIfNotFound: true);
            if (setting == null)
                return null;

            return setting.GetLocalized(x => x.Value, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }

        /// <summary>
        /// Save localized property of setting
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="value">Localizaed value</param>
        /// <returns>Localized property</returns>
        public static void SaveLocalizedSetting<T>(this T settings, Expression<Func<T, string>> keySelector,
            int languageId, string value) where T : ISettings, new()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            var localizedEntityService = EngineContext.Current.Resolve<ILocalizedEntityService>();

            var key = settings.GetSettingKey(keySelector);

            //we do not support localized settings per store (overridden store settings)
            var setting = settingService.GetSetting(key, storeId: 0, loadSharedValueIfNotFound: false);
            if (setting == null)
                return;

            localizedEntityService.SaveLocalizedValue(setting, x => x.Value, value, languageId);
        }
    }
}
