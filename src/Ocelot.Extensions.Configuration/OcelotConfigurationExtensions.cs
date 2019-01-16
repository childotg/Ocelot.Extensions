using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;


namespace Ocelot.DependencyInjection
{
    public static class OcelotConfigurationExtensions
    {
        public static IOcelotBuilder WithConfigurationRepository(this IOcelotBuilder builder,string configKey= "Ocelot.Extensions:Configuration")
        {
            var config = new OcelotConfiguration();
            builder.Configuration.Bind(configKey, config);
            builder.Services.AddSingleton(config);            
            builder.Services.AddHostedService<OcelotConfigurationSyncAgent>();
            return builder;
        }

    }
}
