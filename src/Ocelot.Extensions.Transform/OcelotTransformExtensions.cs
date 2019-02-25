using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.Extensions.Common;
using Ocelot.Extensions.Common.Repository;
using Ocelot.Extensions.Transform;

namespace Ocelot.DependencyInjection
{
    public static class OcelotTransformExtensions
    {
        public static IOcelotBuilder WithReplaceHandler(this IOcelotBuilder builder)
        {
            builder
                .WithRouteExtensions();
                //.Services
                //.AddScoped<DelegatingHandler,ReplaceHandler>(
                //        sp => new ReplaceHandler(
                //            sp.GetService<IFileConfigurationRepositoryExtended>(),
                //            sp.GetRequiredService<IHttpContextAccessor>(),
                //            sp.GetService<ILoggerFactory>()));
            builder.AddDelegatingHandler<ReplaceHandler>();
            return builder;
        }

    }
}
