using Nobreak.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nobreak.Context.Entities
{
    public class NobreakState : Entity
    {
        private static readonly Regex Q1Res = new Regex(@"\((\d*.\d*) (\d*.\d*) (\d*.\d*) (\d*) (\d*.\d*) (\d*.\d*) (\d*.\d*) (\d*)");

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
        [NotMapped]
        public float BatteryPercentage =>
            PowerState == PowerStates.Grid
                ? BatteryVoltage.Map(23.8f, 27.4f, 0, 100, true)
                : BatteryVoltage.Map(20.0f, 24.4f, 0, 100, true);

        [Display(Name = "Temperatura")]
        public float TemperatureC { get; set; }

        [Display(Name = "Estado")]
        [NotMapped]
        public PowerStates PowerState
        {
            get => (Extras & 0b10000000) > 0
                ? PowerStates.Battery
                : PowerStates.Grid;
            set
            {
                if (value == PowerStates.Grid)
                    Extras = (byte)(Extras & 0b01111111);
                else
                    Extras = (byte)(Extras | 0b10000000);
            }
        }

        [Display(Name = "Extras")]
        [Column(TypeName = "tinyint unsigned")]
        public byte Extras { get; set; }

        [NotMapped]
        [JsonConverter(typeof(TimespanConverter))]
        [Display(Name = "Há")]
        public TimeSpan TimeAgo =>
            DateTime.Now - Timestamp;

        public NobreakState()
        {
            Timestamp = DateTime.Now;
        }

        public static NobreakState FromSerialResponse(string response)
        {
            var match = Q1Res.Match(response);
            if (!match.Success)
                throw new Exception("Porta serial não retornou com o formato esperado");
            return new NobreakState
            {
                VoltageIn = float.Parse(match.Groups[1].Value),
                VoltageOut = float.Parse(match.Groups[3].Value)*2,
                LoadPercentage = byte.Parse(match.Groups[4].Value),
                FrequencyHz = float.Parse(match.Groups[5].Value),
                BatteryVoltage = float.Parse(match.Groups[6].Value),
                TemperatureC = float.Parse(match.Groups[7].Value),
                Extras = Convert.ToByte(match.Groups[8].Value,2)
            };
        }

        public enum PowerStates
        {
            Grid,
            Battery
        }

        public override string ToString() =>
            $"Id: {Id}; Timestamp: {Timestamp}; Entrada: {VoltageIn}v; Saída: {VoltageOut}v; Potência: {LoadPercentage}%; Frequência: {FrequencyHz}Hz; Bateria: {BatteryVoltage}v; Temperatura: {TemperatureC}ºC; Modo: {PowerState}";
    }
}
