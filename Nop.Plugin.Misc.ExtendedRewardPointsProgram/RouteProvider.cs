using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram
{
    /// <summary>
    /// Represents route provider for extended reward points program
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Misc.ExtendedRewardPointsProgram.RewardPointsOnDateCreateOrUpdate",
                 "Plugins/ExtendedRewardPointsProgram/RewardPointsOnDateCreateOrUpdate",
                 new { controller = "ExtendedRewardPointsProgram", action = "RewardPointsOnDateCreateOrUpdate", },
                 new[] { "Nop.Plugin.Misc.ExtendedRewardPointsProgram.Controllers" }
            );
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
