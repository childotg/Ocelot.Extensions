using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Extensions.Common;
using Ocelot.Responses;

namespace Ocelot.Extensions.Common.Repository
{
    public class DiskFileConfigurationRepositoryExtended : IFileConfigurationRepository, IFileConfigurationRepositoryExtended
    {
        private readonly string _environmentFilePath;
        private readonly string _ocelotFilePath;
        private static readonly object _lock = new object();
        

        public DiskFileConfigurationRepositoryExtended(IHostingEnvironment hostingEnvironment)
        {
            _environmentFilePath = $"{AppContext.BaseDirectory}{Project.ConfigurationFileName}"
                + $"{(string.IsNullOrEmpty(hostingEnvironment.EnvironmentName) ? string.Empty : ".")}{hostingEnvironment.EnvironmentName}"
                + ".json";
            _ocelotFilePath = $"{AppContext.BaseDirectory}{Project.ConfigurationFileName}.json";
        }

        public FileConfigurationExtended GetExtended()
        {
            string jsonConfiguration;
            lock (_lock)
            {
                jsonConfiguration = System.IO.File.ReadAllText(_ocelotFilePath);
            }
            var fileConfiguration = JsonConvert.DeserializeObject<FileConfigurationExtended>(jsonConfiguration);
            return fileConfiguration;
        }

        public void SetExtended(FileConfigurationExtended fileConfiguration)
        {
            string jsonConfiguration = JsonConvert.SerializeObject(fileConfiguration, Formatting.Indented);            

            lock (_lock)
            {
                if (System.IO.File.Exists(_environmentFilePath))
                {
                    System.IO.File.Delete(_environmentFilePath);
                }
                System.IO.File.WriteAllText(_environmentFilePath, jsonConfiguration);

                if (System.IO.File.Exists(_ocelotFilePath))
                {
                    System.IO.File.Delete(_ocelotFilePath);
                }
                System.IO.File.WriteAllText(_ocelotFilePath, jsonConfiguration);
            }
        }

        public Task<Response<FileConfiguration>> Get()
        {
            return Task.FromResult<Response<FileConfiguration>>(new OkResponse<FileConfiguration>(GetExtended()));
        }

        public Task<Response> Set(FileConfiguration fileConfiguration)
        {
            SetExtended(new FileConfigurationExtended(fileConfiguration));
            return Task.FromResult<Response>(new OkResponse());
        }
    }
}
