using Nobreak.Context.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nobreak.Models
{
    public class NobreakStateChangeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "De propósito")]
        public bool OnPurpose { get; set; }

        public PowerStates PowerState { get; set; }

        [NotMapped]
        [Display(Name = "Duração")]
        public TimeSpan Duration { get; set; }

        [Required]
        public NobreakStateViewModel NobreakState { get; set; }

        public override string ToString() =>
            $"{NobreakState}; De propósito: {OnPurpose.SimNao()}; Duração: {Duration.Format()}";
    }
}
