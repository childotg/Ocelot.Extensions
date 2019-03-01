using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Extensions.Common.Repository;
using Ocelot.Extensions.Configuration;
using Ocelot.Extensions.Configuration.Repository;
using System;
using System.Collections.Generic;
using System.Text;


namespace Ocelot.DependencyInjection
{
    public static class OcelotConfigurationExtensions
    {
        private static bool _withConfigurationRepository = false;
        public static IOcelotBuilder WithConfigurationRepository(this IOcelotBuilder builder,string configKey= "Ocelot.Extensions:Configuration")
        {
            if (!_withConfigurationRepository)
            {
                builder.Services.AddHttpClient();
                builder.WithRouteExtensions();
                var config = new OcelotConfiguration();
                builder.Configuration.Bind(configKey, config);
                builder.Services.AddSingleton(config);
                builder.Services.AddSingleton<IFileConfigurationRepositoryExtended>(
                    sp=>
                        new SnapshotConfigurationRepositoryExtended(
                            new DiskFileConfigurationRepositoryExtended(sp.GetRequiredService<IHostingEnvironment>())));
                builder.Services.AddHostedService<OcelotConfigurationSyncAgent>();
                _withConfigurationRepository = true;
            }
            return builder;
        }

    }
}
