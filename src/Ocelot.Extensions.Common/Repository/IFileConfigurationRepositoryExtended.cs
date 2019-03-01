using System.Threading.Tasks;
using Ocelot.Extensions.Common;
using Ocelot.Responses;

namespace Ocelot.Extensions.Common.Repository
{
    public interface IFileConfigurationRepositoryExtended
    {
        FileConfigurationExtended GetExtended();
        
        void SetExtended(FileConfigurationExtended fileConfiguration);
        
    }
}