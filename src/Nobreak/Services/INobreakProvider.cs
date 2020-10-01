using Nobreak.Context.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Services
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
