using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Extensions.Configuration.AzureStorage
{
    public class AzureStorageConfiguration
    {
        public AzureStorageConfigurationType Type { get; set; }
        public AzureStorageConfigurationAccessType AccessType { get; set; }
        public string ResourceUri { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
    }

    public enum AzureStorageConfigurationType
    {
        Blob,
        File
    }

    public enum AzureStorageConfigurationAccessType
    {
        SharedKey,
        SignedUri
    }
}
