using Nobreak.Context.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Models
{
    public class UptimeStateViewModel
    {
        [Display(Name = "Estado")]
        public PowerStates PowerState { get; set; }

        [Display(Name = "Porção")]
        public TimeSpan ShareTimespan { get; set; }

        [Display(Name = "Porção")]
        public double SharePercentage { get; set; }
    }
}
