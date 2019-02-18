using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Extensions.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Extensions.Configuration
{
    public class OcelotConfigurationSyncResult:OcelotConfigurationContentRaw
    {
        public OcelotConfigurationSyncResult(OcelotConfigurationContentRaw inner):base(inner.Result,inner.Version)
        {

        }

        public OcelotConfigurationSyncResult()
        {

        }

        public FileConfigurationExtended Configuration
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<FileConfigurationExtended>(Result);
                }
                catch (Exception ex)
                {
                    SyncErrorException = ex;
                    return null;
                }               
            }
        }


        public bool HasResult {
            get
            {
                return !string.IsNullOrWhiteSpace(Result) && !SyncError;
            }
        }
        public bool SyncError
        {
            get
            {
                return Configuration==null || SyncErrorException != null;
            }
        }
        public Exception SyncErrorException { get; set; }        
        
    }

   
}
