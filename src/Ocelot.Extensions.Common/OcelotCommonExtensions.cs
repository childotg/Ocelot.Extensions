using Ocelot.Middleware;
using Ocelot.Extensions.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Ocelot.Extensions.Common.Repository;
using Microsoft.Extensions.Configuration;
using Ocelot.Configuration.Repository;

namespace Ocelot.DependencyInjection
{

    public static class OcelotCommonExtensions
    {
        private static bool _initialized = false;
        public static IOcelotBuilder WithRouteExtensions(this IOcelotBuilder builder, string configKey = "RouteExtensions")
        {
            if (!_initialized)
            {
                builder.Services.AddSingleton<IFileConfigurationRepository, DiskFileConfigurationRepositoryExtended>();
                builder.Services.AddSingleton<IFileConfigurationRepositoryExtended, DiskFileConfigurationRepositoryExtended>();
                builder.Services.Configure<RouteExtensions>(builder.Configuration.GetSection(configKey));
                _initialized = true;
            }
            return builder;
        }

        private static bool _usedOcelotExtended = false;
        public static async Task<IApplicationBuilder> UseOcelotExtended(this IApplicationBuilder builder, OcelotPipelineConfiguration pipelineConfiguration=null)
        {
            if (!_usedOcelotExtended)
            {
                OcelotPipelineConfiguration conf = pipelineConfiguration ?? new OcelotPipelineConfiguration();
                conf.PreAuthenticationMiddleware = async (ctx, next) =>
                {
                    ctx.HttpContext.Items["RouteKey"] = ctx.DownstreamReRoute.Key;
                    await next.Invoke();
                };
                var result=await builder.UseOcelot(conf);
                _usedOcelotExtended = true;
                return result;
            }
            else return builder;
            
        }

        //public static IOcelotBuilder AddOcelotExtended(this IServiceCollection services, IConfiguration configuration=null)
        //{
        //    services.AddOcelot(configuration)
        //        .wi
        //}
        
    }
}
