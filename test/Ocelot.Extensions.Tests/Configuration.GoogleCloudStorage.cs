using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Extensions.Configuration.AzureStorage;
using Ocelot.Extensions.Configuration.GoogleCloudStorage;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Ocelot.Extensions.Tests
{
    public class Configuration_GoogleCloudStorage
    {
        private const string Google_ValidOcelotConfigurationAccessKey = "";
        private const string Google_ValidOcelotConfigurationSecret = "";
        private const string Google_ValidOcelotConfigurationBucketname = "";
        private const string Google_ValidOcelotConfigurationObjectName = "";

        private Func<GoogleCloudStorageConfiguration> Google_ValidSourceWithSharedKey { get; } =
           () => new GoogleCloudStorageConfiguration()
           {
               AccessKey = Google_ValidOcelotConfigurationAccessKey,
               BucketName = Google_ValidOcelotConfigurationBucketname,
               ObjectName = Google_ValidOcelotConfigurationObjectName,
               Secret = Google_ValidOcelotConfigurationSecret
           };

        [Fact]
        public async Task ValidFile_UsingGoogleWithSharedKey_ShouldReturnValidConfiguration()
        {
            var instance = new GoogleCloudStorageService(Google_ValidSourceWithSharedKey(), NullLoggerFactory.Instance);
            var content = await instance.FetchData(null);
            Assert.True(content.HasResult, "Error while fetching data from a SignedUri");
            Assert.True(content.Configuration != null, "Error while deserializing data from a SignedUri");
        }
    }
}
