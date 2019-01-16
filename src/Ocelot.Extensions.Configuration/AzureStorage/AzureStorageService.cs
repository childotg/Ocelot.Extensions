using Microsoft.Extensions.Logging;
using Ocelot.Extensions.Configuration.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ocelot.Extensions.Configuration.AzureStorage
{
    public class AzureStorageService : OcelotConfigurationFetchServiceBase
    {
        private readonly AzureStorageConfiguration _config;
        
        public AzureStorageService(AzureStorageConfiguration config, ILoggerFactory loggerFactory)
        {
            this._config = config;
            this._logger = loggerFactory.CreateLogger<AzureStorageService>();
        }


        protected override Task<OcelotConfigurationContentRaw> FetchDataImpl(string latestVersion)
        {
            string signedUri = null;
            var type = _config.Type == AzureStorageConfigurationType.Blob ? "blob" : "file";
            switch (_config.AccessType)
            {
                case AzureStorageConfigurationAccessType.SharedKey:
                    signedUri = SigningUtils.SignUriForAzureStorage(_config.AccountName, _config.AccountKey, _config.ResourceUri, type);
                    break;
                case AzureStorageConfigurationAccessType.SignedUri:
                    signedUri = _config.ResourceUri;
                    break;
                default:
                    throw new InvalidOperationException($"AccessType {_config.AccessType} is not valid");
            }

            return GetWithHttpAndETAG(latestVersion, signedUri);
        }

        


    }
}
