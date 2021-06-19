using Nobreak.Context.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Nobreak.Models
{
    public class NobreakStateViewModel
    {
        [Display(Name = "Ocorreu em")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Entrada")]
        public float VoltageIn { get; set; }

        [Display(Name = "Saída")]
        public float VoltageOut { get; set; }

        [Display(Name = "Potência")]
        public byte LoadPercentage { get; set; }

        [Display(Name = "Frequência")]
        public float FrequencyHz { get; set; }

        [Display(Name = "Bateria")]
        public float BatteryVoltage { get; set; }

        [Display(Name = "Bateria")]
        public float BatteryPercentage { get; set; }

        [Display(Name = "Temperatura")]
        public float TemperatureC { get; set; }

        [Display(Name = "Estado")]
        public PowerStates PowerState { get; set; }


        [Display(Name = "Extras")]
        public byte Extras { get; set; }

        [JsonIgnore]
        [Display(Name = "Há")]
        public TimeSpan TimeAgo =>
            DateTime.UtcNow - Timestamp;
    }
}
