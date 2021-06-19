using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Nobreak.Context.Entities
{
    public class NobreakState : Entity
    {
        private static readonly Regex _q1Res = new Regex(@"\((\d*.\d*) (\d*.\d*) (\d*.\d*) (\d*) (\d*.\d*) (\d*.\d*) (\d*.\d*) (\d*)");

        public float VoltageIn { get; set; }

        public float VoltageOut { get; set; }

        public byte LoadPercentage { get; set; }

        public float FrequencyHz { get; set; }

        public float BatteryVoltage { get; set; }

        [NotMapped]
        public float BatteryPercentage =>
            PowerState == PowerStates.Grid
                ? BatteryVoltage.Map(23.8f, 26.99f, 0, 100, true)
                : BatteryVoltage.Map(20.0f, 24.4f, 0, 100, true);

        public float TemperatureC { get; set; }

        [NotMapped]
        public PowerStates PowerState
        {
            get => (Extras & 0b10000000) > 0
                ? PowerStates.Battery
                : PowerStates.Grid;
            set
            {
                if (value == PowerStates.Grid)
                    Extras = (byte) (Extras & 0b01111111);
                else
                    Extras = (byte) (Extras | 0b10000000);
            }
        }

        [Column(TypeName = "tinyint unsigned")]
        public byte Extras { get; set; }

        [NotMapped]
        public TimeSpan TimeAgo =>
            DateTime.Now - Timestamp;

        protected NobreakState() { }

        public NobreakState(PowerStates powerState, DateTime timestamp)
        {
            Timestamp = timestamp;
            PowerState = powerState;
        }

        public static NobreakState FromSerialResponse(string response)
        {
            var match = _q1Res.Match(response);
            if (!match.Success)
                throw new Exception("Porta serial não retornou com o formato esperado");
            return new NobreakState
            {
                VoltageIn = float.Parse(match.Groups[1].Value),
                VoltageOut = float.Parse(match.Groups[3].Value) * 2,
                LoadPercentage = byte.Parse(match.Groups[4].Value),
                FrequencyHz = float.Parse(match.Groups[5].Value),
                BatteryVoltage = float.Parse(match.Groups[6].Value),
                TemperatureC = float.Parse(match.Groups[7].Value),
                Extras = Convert.ToByte(match.Groups[8].Value, 2)
            };
        }

        public override string ToString() =>
            $"Id: {Id}; Timestamp: {Timestamp}; Entrada: {VoltageIn}v; Saída: {VoltageOut}v; Potência: {LoadPercentage}%; Frequência: {FrequencyHz}Hz; Bateria: {BatteryVoltage}v; Temperatura: {TemperatureC}ºC; Modo: {PowerState}";
    }
}
