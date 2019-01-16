using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Extensions.Configuration
{
    public enum OcelotConfigurationRepositoryType
    {
        Local = 0, //To be implemented        
        AzureStorage = 10,                
        GoogleCloudStorage = 20, 
        AmazonS3 = 30, //To be implemented
        Redis = 40, //To be implemented
        RelationalDatabase = 50, //To be implemented
        PlainHTTP = 60, //To be implemented
    }
}
