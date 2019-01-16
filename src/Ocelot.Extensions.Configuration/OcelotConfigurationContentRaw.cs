using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Extensions.Configuration
{
    public class OcelotConfigurationContentRaw
    {
        public OcelotConfigurationContentRaw()
        {

        }
        public OcelotConfigurationContentRaw(string result, string version)
        {
            Result = result;
            Version = version;
        }

        public string Result { get; set; }
        public string Version { get; set; }
    }
}
