using Microsoft.Extensions.Logging;
using Ocelot.Extensions.Configuration.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ocelot.Extensions.Configuration.GoogleCloudStorage
{
    
    public class GoogleCloudStorageService : OcelotConfigurationFetchServiceBase
    {
        private readonly GoogleCloudStorageConfiguration _config;
        
        public GoogleCloudStorageService(GoogleCloudStorageConfiguration config, ILoggerFactory loggerFactory)
        {
            this._config = config;
            this._logger = loggerFactory.CreateLogger<GoogleCloudStorageService>();
        }


        protected override Task<OcelotConfigurationContentRaw> FetchDataImpl(string latestVersion)
        {
            var signedUri = SigningUtils
                .SignUriForGoogleCloudStorage(_config.BucketName, _config.ObjectName, _config.AccessKey, _config.Secret);
            return GetWithHttpAndETAG(latestVersion, signedUri);
        }

        

    }
}
