using Nobreak.Context.Entities;
using Nobreak.Infra.Services.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Models
{
    public class UptimeReportViewModel
    {
        public List<NobreakStateChangeViewModel> StateChanges { get; set; }

        public List<UptimeInIntervalViewModel> UptimePerIntervals { get; set; }

        public List<PowerStates> PossibleStates { get; set; }
    }
}
