using Nobreak.Context.Entities;
using System.Collections.Generic;

namespace Nobreak.Models
{
    public class UptimeReportViewModel
    {
        public List<NobreakStateChangeViewModel> StateChanges { get; set; }

        public List<UptimeInIntervalViewModel> UptimePerIntervals { get; set; }

        public List<PowerStates> PossibleStates { get; set; }
    }
}
