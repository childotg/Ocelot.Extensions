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
    public class Configuration_AzureStorage
    {
        private const string Azure_ValidOcelotConfigurationSignedUri = "";
        private const string InvalidURL = "";
        private const string Azure_InvalidJsonSignedUri = "";
        private const string Azure_ValidOcelotConfigurationBlobName = "";
        private const string Azure_ValidOcelotConfigurationAccountName = "";
        private const string Azure_ValidOcelotConfigurationAccountKey = "";
        

        private Func<AzureStorageConfiguration> ValidSourceWithSignedUri { get; } = 
            () => new AzureStorageConfiguration()
                {
                    AccessType = AzureStorageConfigurationAccessType.SignedUri,
                    Type = AzureStorageConfigurationType.Blob,
                    ResourceUri = Azure_ValidOcelotConfigurationSignedUri
            };

        private Func<AzureStorageConfiguration> MissingSource { get; } =
            () => new AzureStorageConfiguration()
            {
                AccessType = AzureStorageConfigurationAccessType.SignedUri,
                Type = AzureStorageConfigurationType.Blob,
                ResourceUri = InvalidURL                
            };

        private Func<AzureStorageConfiguration> InvalidSource { get; } =
           () => new AzureStorageConfiguration()
           {
               AccessType = AzureStorageConfigurationAccessType.SignedUri,
               Type = AzureStorageConfigurationType.Blob,
               ResourceUri = Azure_InvalidJsonSignedUri
           };

        

        private Func<AzureStorageConfiguration> ValidSourceWithSharedKey { get; } =
            () => new AzureStorageConfiguration()
            {
                AccessType = AzureStorageConfigurationAccessType.SharedKey,
                Type = AzureStorageConfigurationType.Blob,
                ResourceUri = Azure_ValidOcelotConfigurationBlobName,
                AccountName= Azure_ValidOcelotConfigurationAccountName,
                AccountKey= Azure_ValidOcelotConfigurationAccountKey
            };

       

        [Fact]
        public async Task ValidFile_UsingBlobWithSignedUri_ShouldReturnValidConfiguration()
        {            
            var instance = new AzureStorageService(ValidSourceWithSignedUri(), NullLoggerFactory.Instance);
            var content = await instance.FetchData(null);
            Assert.True(content.HasResult, "Error while fetching data from a SignedUri");
            Assert.True(content.Configuration != null, "Error while deserializing data from a SignedUri");
        }

        [Fact]
        public async Task ValidFile_ConsecutiveCalls_ShouldSkipSecondFetch()
        {
            var instance = new AzureStorageService(ValidSourceWithSignedUri(), NullLoggerFactory.Instance);
            var content1 = await instance.FetchData(null);
            Assert.True(content1.HasResult, "Invalid object detected");
            var content2 = await instance.FetchData(content1.Version);
            Assert.False(content2.HasResult, "Fetch must not have place is same version is detected");
        }

        [Fact]
        public async Task MissingFile_UsingBlobWithSignedUri_ShouldReturnError()
        {
            var instance = new AzureStorageService(MissingSource(), NullLoggerFactory.Instance);
            var content = await instance.FetchData(null);
            Assert.True(content.SyncError && !content.HasResult, "Response must be invalid");            
        }

        [Fact]
        public async Task InvalidFile_UsingBlobWithSignedUri_ShouldReturnError()
        {
            var instance = new AzureStorageService(InvalidSource(), NullLoggerFactory.Instance);
            var content = await instance.FetchData(null);
            Assert.True(content.SyncError && !content.HasResult, "Response must be invalid");
        }


        [Fact]
        public async Task ValidFile_UsingBlobWithSharedKey_ShouldReturnValidConfiguration()
        {
            var instance = new AzureStorageService(ValidSourceWithSharedKey(), NullLoggerFactory.Instance);
            var content = await instance.FetchData(null);
            Assert.True(content.HasResult, "Error while fetching data from a SignedUri");
            Assert.True(content.Configuration != null, "Error while deserializing data from a SignedUri");
        }

        
    }
}
