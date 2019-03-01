using System.Linq;
using Newtonsoft.Json;
using Ocelot.Extensions.Common;
using Ocelot.Extensions.Common.Repository;
using Ocelot.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ocelot.Extensions.Configuration.Repository
{
    public class SnapshotConfigurationRepositoryExtended: IFileConfigurationRepositoryExtended
    {
        private readonly IFileConfigurationRepositoryExtended _innerRepository;
        private string _snapShotsPath;
        public SnapshotConfigurationRepositoryExtended(IFileConfigurationRepositoryExtended inner)
        {
            this._innerRepository = inner;
            this._snapShotsPath = Path.Combine(AppContext.BaseDirectory, Project.ProjectName, "Configuration", "Snapshots");
            if (!Directory.Exists(_snapShotsPath)) Directory.CreateDirectory(_snapShotsPath);
        }

        public FileConfigurationExtended GetExtended()
        {
            try
            {
                return _innerRepository.GetExtended();
            }
            catch (Exception ex)
            {
                //Try look into backups
                var snapShots = Directory.GetFiles(_snapShotsPath, "*.json");
                if (snapShots.Any())
                {
                    var contents = File.ReadAllText(snapShots.Last());
                    return JsonConvert.DeserializeObject<FileConfigurationExtended>(contents);
                }
                else throw ex;
            }
        }

        public void SetExtended(FileConfigurationExtended fileConfiguration)
        {
            try
            {
                var config = GetExtended();
                if (config == null) throw new ArgumentNullException();

                var now = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
                var backup = Path.Combine(_snapShotsPath, $"{Project.ConfigurationFileName}-{now}.json");
                File.WriteAllText(backup, JsonConvert.SerializeObject(config));
            }
            finally
            {
                _innerRepository.SetExtended(fileConfiguration);
            }
        }
    }
}
