using Nobreak.Context.Entities;
using System;
using System.ComponentModel.DataAnnotations;

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
