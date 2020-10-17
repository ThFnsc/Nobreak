using Nobreak.Context.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nobreak.Infra.Context.Entities
{
    public class NobreakStateChange : Entity
    {
        [Display(Name = "De propósito")]
        public bool OnPurpose { get; set; }

        public PowerStates PowerState =>
            OnPurpose ? PowerStates.Grid : NobreakState.PowerState;

        [NotMapped]
        [Display(Name = "Duração")]
        public TimeSpan Duration { get; set; }

        [Required]
        public NobreakState NobreakState { get; set; }

        protected NobreakStateChange() { }

        public NobreakStateChange(NobreakState nobreakState)
        {
            NobreakState = nobreakState;
        }

        public override string ToString() =>
            $"{NobreakState}; De propósito: {OnPurpose.SimNao()}; Duração: {Duration.Format()}";
    }
}
