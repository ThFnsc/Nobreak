using Nobreak.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Context.Entities
{
    public class NobreakStateChange : Entity
    {
        [Display(Name = "De propósito")]
        public bool OnPurpose { get; set; }

        public NobreakState.PowerStates PowerState =>
            OnPurpose ? NobreakState.PowerStates.Grid : NobreakState.PowerState;

        [NotMapped]
        [JsonConverter(typeof(TimespanConverter))]
        public TimeSpan Duration { get; set; }

        [Required]
        public NobreakState NobreakState { get; set; }

        public override string ToString() =>
            $"{NobreakState}; De propósito: {OnPurpose.SimNao()}; Duração: {Duration.Format()}";
    }
}
