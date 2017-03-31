using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Data;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Domain;
using Nop.Plugin.Misc.ExtendedRewardPointsProgram.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram.Infrastructure
{
    /// <summary>
    /// Represents dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //register override services
            builder.RegisterType<ExtendedRewardPointsProgramSettingService>().As<ISettingService>().InstancePerLifetimeScope();

            //register own service
            builder.RegisterType<RewardPointsOnDateSettingsService>().As<IRewardPointsOnDateSettingsService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<ExtendedRewardPointsProgramObjectContext>(builder, "nop_object_context_extended_reward_points_program");

            //override required repository with custom context
            builder.RegisterType<EfRepository<RewardPointsOnDateSettings>>().As<IRepository<RewardPointsOnDateSettings>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_extended_reward_points_program")).InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}
