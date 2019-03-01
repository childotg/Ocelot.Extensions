using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Setter;
using Ocelot.Extensions.Common.Repository;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Extensions.Configuration
{   
    public class OcelotConfigurationSyncAgent : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly OcelotConfiguration _config;
        private readonly IFileConfigurationRepositoryExtended _setter;
        private readonly IHttpClientFactory _httpClientFactory;
        

        public OcelotConfigurationSyncAgent(OcelotConfiguration config, IFileConfigurationRepositoryExtended configSetter
            , ILoggerFactory loggerFactory,IHttpClientFactory factory)
        {
            this._setter = configSetter;
            this._config = config;
            this._loggerFactory = loggerFactory;
            this._logger = loggerFactory.CreateLogger<OcelotConfigurationSyncAgent>();
            this._httpClientFactory = factory;
        }
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IOcelotConfigurationFetchService instance = null;
            try
            {
                _logger.LogInformation("Starting the background service...");
                var type = _config.RepositoryType.ToString();
                _logger.LogInformation($"Using the {type} repository...");
                var ns = _config.GetType().Namespace;
                var configSection = _config.GetType().GetProperty(type).GetValue(_config);
                instance = Activator.CreateInstance(Type.GetType($"{ns}.{type}.{type}Service"), configSection, _loggerFactory,_httpClientFactory)
                    as IOcelotConfigurationFetchService;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Background service cannot initialize!");
                return;
            }

            OcelotConfigurationSyncResult result = null;
            _logger.LogInformation("Entering the continous polling process...");

            int retryAttempts = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var previousVersion = result?.Version ?? "new";
                _logger.LogTrace($"Pre-fetch with version {previousVersion}");
                result = await instance.FetchData(previousVersion);
                if (result.HasResult)
                {
                    _logger.LogTrace($"Version {previousVersion} transitioned to version {result.Version}");
                    //retryAttempts = 0;
                    _setter.SetExtended(result.Configuration);
                    _logger.LogTrace($"Ocelot configuration updated");
                }
                else
                {
                    if (result.SyncErrorException != null) _logger.LogError(result.SyncErrorException, "Error while fetching new data!");
                    //retryAttempts++;
                }
                
                int nextIterationInMilliseconds = _config.CheckingInterval * (1 + retryAttempts) * 1000;
                await Task.Delay(nextIterationInMilliseconds, stoppingToken);
            }

        }

        
    }
}
