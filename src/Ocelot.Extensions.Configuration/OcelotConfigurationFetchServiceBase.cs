using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ocelot.Extensions.Configuration
{
    public abstract class OcelotConfigurationFetchServiceBase : IOcelotConfigurationFetchService
    {
        protected ILogger _logger;
        
        protected abstract Task<OcelotConfigurationContentRaw> FetchDataImpl(string latestVersion);
        public async Task<OcelotConfigurationSyncResult> FetchData(string latestVersion)
        {
            OcelotConfigurationSyncResult result = null;
            try
            {
                result=new OcelotConfigurationSyncResult(await FetchDataImpl(latestVersion));
                return result;
            }
            catch (Exception ex)
            {                
                return new OcelotConfigurationSyncResult()
                {                    
                    SyncErrorException = ex
                };
            }
        }

        protected async Task<OcelotConfigurationContentRaw> GetWithHttpAndETAG(string latestVersion, string signedUri)
        {
            var client = new HttpClient();
            var head = new HttpRequestMessage(HttpMethod.Head, signedUri);
            _logger.LogTrace($"Looking with HEAD at the resource {signedUri} with version {latestVersion}");
            var headers = await client.SendAsync(head);

            if (headers.IsSuccessStatusCode)
            {
                if (latestVersion == null || (headers.Headers.ETag.Tag != latestVersion))
                {
                    var newVersion = headers.Headers.ETag.Tag;
                    _logger.LogTrace($"Taking most recent version {newVersion} of configuration...");
                    var request = new HttpRequestMessage(HttpMethod.Get, signedUri);
                    var response = await client.SendAsync(request);
                    if (response.Content != null)
                    {                        
                        var content = await response.Content.ReadAsStringAsync();
                        return new OcelotConfigurationContentRaw(content, headers.Headers.ETag.Tag);
                    }
                    else throw new InvalidOperationException("Content is empty!");
                }
                else return new OcelotConfigurationContentRaw("", latestVersion);
            }
            else throw new InvalidOperationException("HTTP resource missing, wrong auth settings or invalid URI");
        }
    }
}
