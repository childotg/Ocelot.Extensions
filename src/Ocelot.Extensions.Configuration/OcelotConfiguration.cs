using Ocelot.Extensions.Configuration.AzureStorage;
using Ocelot.Extensions.Configuration.GoogleCloudStorage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Extensions.Configuration
{
    
    public class OcelotConfiguration
    {        
        public OcelotConfigurationRepositoryType RepositoryType { get; set; }
        public AzureStorageConfiguration AzureStorage { get; set; }
        public GoogleCloudStorageConfiguration GoogleCloudStorage { get; set; }
        public int CheckingInterval { get; set; }
    }

    
}
