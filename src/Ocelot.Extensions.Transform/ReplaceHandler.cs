using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.Extensions.Common;
using Ocelot.Extensions.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ocelot.Extensions.Transform
{
    public class ReplaceHandler : DelegatingHandler
    {
        //private IOptionsMonitor<RouteExtensions> configuration = null;
        private IHttpContextAccessor httpContext = null;
        private ILogger _logger = null;
        private readonly IFileConfigurationRepositoryExtended _config = null;
        public ReplaceHandler(IFileConfigurationRepositoryExtended config, 
            IHttpContextAccessor accessor,ILoggerFactory loggerFactory)
        {
            this.httpContext = accessor;
            this._config = config;            
            this._logger = loggerFactory.CreateLogger<ReplaceHandler>();
        }

        private bool IsSupportedMediaType(string mediaType)
        {
            return !string.IsNullOrWhiteSpace(mediaType) && (mediaType.Contains("text") || mediaType.Contains("json") || mediaType.Contains("xml"));
        }

        private string HeaderValue(IEnumerable<string> values)
        {
            if (values != null && values.Any())
                return string.Join(",", values.ToArray());
            else return "";
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var configuration = _config.GetExtended();
                var routeKey = httpContext.HttpContext.Items["RouteKey"];
                if (routeKey == null) return await base.SendAsync(request, cancellationToken);

                var currentConf = configuration.RouteExtensions.ReplaceHandler.Rules
                    .Where(p => p.AppliesTo != null && p.AppliesTo.Contains(routeKey))
                    .FirstOrDefault();

                if (currentConf != null)
                {
                    //Downstream headers
                    var headers = currentConf.ReplaceDownstreamHeaders
                        .GroupBy(p => p.Header).ToDictionary(p => p.Key, p => p);
                    foreach (var header in headers)
                    {
                        if (request.Headers.Contains(header.Key))
                        {
                            var values = request.Headers.GetValues(header.Key);
                            var newValues = new List<string>();
                            foreach (var val in values)
                            {
                                var value = new StringBuilder(val);                                
                                foreach (var toReplace in header.Value)
                                {
                                    value.Replace(toReplace.Find, toReplace.Replace);
                                }
                                newValues.Add(value.ToString());
                            }
                            request.Headers.Remove(header.Key);
                            request.Headers.Add(header.Key, newValues);
                        }
                    }
                    //Disable compression
                    request.Headers.Remove("Accept-Encoding");

                    //Downstream content                
                    if (request.Content != null && IsSupportedMediaType(request.Content?.Headers?.ContentType?.MediaType))
                    {
                        var content = new StringBuilder(await request.Content.ReadAsStringAsync());
                        foreach (var toReplace in currentConf.ReplaceDownstreamContent)
                        {
                            content.Replace(toReplace.Find, toReplace.Replace);
                        }
                        request.Content = new StringContent(content.ToString(), Encoding.UTF8, request.Content.Headers.ContentType.MediaType);
                    }                    

                    //Go with request
                    var response = await base.SendAsync(request, cancellationToken);

                    if (response.IsSuccessStatusCode && IsSupportedMediaType(response.Content?.Headers?.ContentType?.MediaType))
                    {
                        //Upstream content
                        var content = await response.Content.ReadAsStringAsync();    
                        
                        var result = new StringBuilder(content);
                        foreach (var toReplace in currentConf.ReplaceUpstreamContent)
                        {
                            result.Replace(toReplace.Find, toReplace.Replace);
                        }
                        response.Content = new StringContent(result.ToString(), Encoding.UTF8, response.Content.Headers.ContentType.MediaType);
                    }

                    return response;
                }

                else return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }


        }
    }
}
