using System;
using System.Linq.Expressions;
using System.Reflection;
using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services
{
    /// <summary>
    /// Represents custom setting extensions
    /// </summary>
    public static class SettingExtensions
    {
        /// <summary>
        /// Get setting key (stored into database)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Key</returns>
        public static string GetSettingKey<T, TPropType>(this T entity, Expression<Func<T, TPropType>> keySelector) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", keySelector));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", keySelector));

            //for settings of plugin get runtime type; otherwise get compile type
            var compileType = typeof(T);
            var runtimeType = compileType != typeof(RewardPointsSettings) ? compileType : entity.GetType();

            return string.Format("{0}.{1}", runtimeType.Name, propInfo.Name);
        }
    }
}
