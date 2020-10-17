using Nobreak.Context.Entities;
using Nobreak.Infra.Context.Entities;
using Nobreak.Infra.Services.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Infra.Services
{
    public interface INobreakProvider
    {
        Task<UptimeReport> GetUptimeReportAsync();

        Task<List<NobreakState>> GetRecentValuesAsync();

        Task GetAllValuesAsync(Stream writeTo);

        void ClearCache();

        Task<NobreakStateChange> ToggleOnPurposeAsync(int id);
    }
}
