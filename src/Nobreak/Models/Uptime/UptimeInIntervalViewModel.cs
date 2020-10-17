using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Models
{
    public class UptimeInIntervalViewModel
    {
        [Display(Name = "Desde")]
        public DateTime Since { get; set; }

        [Display(Name = "Ocorreu em")]
        public TimeSpan TimeSpan { get; set; }

        [Display(Name = "Estado")]
        public List<UptimeStateViewModel> UptimeStates { get; set; }
    }
}
