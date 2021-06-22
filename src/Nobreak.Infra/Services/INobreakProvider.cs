using Nobreak.Infra.Context.Entities;
using Nobreak.Infra.Services.Report;
using System.IO;
using System.Threading.Tasks;

namespace Nobreak.Infra.Services
{
    public interface INobreakProvider
    {
        Task<UptimeReport> GetUptimeReportAsync();

        Task GetAllValuesAsync(Stream writeTo);

        Task<NobreakStateChange> ToggleOnPurposeAsync(int id);
    }
}
