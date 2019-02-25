using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Extensions.Common;
using Ocelot.Responses;

namespace Ocelot.Extensions.Common.Repository
{
    public class DiskFileConfigurationRepositoryExtended : DiskFileConfigurationRepository, IFileConfigurationRepositoryExtended
    {
        private readonly string _environmentFilePath;
        private readonly string _ocelotFilePath;
        private static readonly object _lock = new object();
        private const string ConfigurationFileName = "ocelot";

        public DiskFileConfigurationRepositoryExtended(IHostingEnvironment hostingEnvironment):base(hostingEnvironment)
        {
            _environmentFilePath = $"{AppContext.BaseDirectory}{ConfigurationFileName}"
                //+ $"{(string.IsNullOrEmpty(hostingEnvironment.EnvironmentName) ? string.Empty : ".")}{hostingEnvironment.EnvironmentName}"
                +".json";

            _ocelotFilePath = $"{AppContext.BaseDirectory}{ConfigurationFileName}.json";
        }

        public Task<Response<FileConfigurationExtended>> GetExtended()
        {
            string jsonConfiguration;

            lock (_lock)
            {
                jsonConfiguration = System.IO.File.ReadAllText(_environmentFilePath);
            }

            var fileConfiguration = JsonConvert.DeserializeObject<FileConfigurationExtended>(jsonConfiguration);

            return Task.FromResult<Response<FileConfigurationExtended>>(new OkResponse<FileConfigurationExtended>(fileConfiguration));
        }

        public Task<Response> SetExtended(FileConfigurationExtended fileConfiguration)
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

            return Task.FromResult<Response>(new OkResponse());
        }
    }
}
