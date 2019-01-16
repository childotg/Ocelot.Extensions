using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ocelot.Extensions.Configuration
{
    public interface IOcelotConfigurationFetchService
    {
        Task<OcelotConfigurationSyncResult> FetchData(string latestVersion);        
    }
}
