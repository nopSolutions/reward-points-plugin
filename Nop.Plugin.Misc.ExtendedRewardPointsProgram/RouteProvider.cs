using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.ExtendedRewardPointsProgram
{
    /// <summary>
    /// Represents route provider for extended reward points program
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Misc.ExtendedRewardPointsProgram.RewardPointsOnDateCreateOrUpdate",
                "Plugins/ExtendedRewardPointsProgram/RewardPointsOnDateCreateOrUpdate",
                new { controller = "ExtendedRewardPointsProgram", action = "RewardPointsOnDateCreateOrUpdate" });
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
