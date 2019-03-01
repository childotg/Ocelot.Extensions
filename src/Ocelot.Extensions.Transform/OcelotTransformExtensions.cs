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
        private static bool _withReplaceHandler = false;
        public static IOcelotBuilder WithReplaceHandler(this IOcelotBuilder builder)
        {
            if (!_withReplaceHandler)
            {
                builder
                    .WithRouteExtensions();                
                builder.AddDelegatingHandler<ReplaceHandler>();
                _withReplaceHandler = true;
            }
            return builder;
        }

    }
}
