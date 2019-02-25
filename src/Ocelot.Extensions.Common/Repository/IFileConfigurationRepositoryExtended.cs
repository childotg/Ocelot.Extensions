using System.Threading.Tasks;
using Ocelot.Extensions.Common;
using Ocelot.Responses;

namespace Ocelot.Extensions.Common.Repository
{
    public interface IFileConfigurationRepositoryExtended
    {
        Task<Response<FileConfigurationExtended>> GetExtended();
        Task<Response> SetExtended(FileConfigurationExtended fileConfiguration);
    }
}