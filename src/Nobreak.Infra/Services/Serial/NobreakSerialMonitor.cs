using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nobreak.Context.Entities;
using Nobreak.Infra.Context;
using Nobreak.Infra.Context.Entities;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nobreak.Infra.Services.Serial
{
    public class NobreakSerialMonitor : TimedHostedService, IDisposable
    {
        private readonly ILogger<NobreakSerialMonitor> _logger;
        private readonly SerialPort _serial;
        private readonly IServiceProvider _serviceProvider;

        public NobreakSerialMonitor(ILogger<NobreakSerialMonitor> logger, IServiceProvider serviceProvider, string serialPortOverride, int? bauldRateOverride) : base(logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            var serialPorts = SerialPort.GetPortNames();
            _logger.LogInformation("Serial ports found: {SerialPorts}", string.Join(", ", serialPorts));

            var chosenSerialPort = serialPortOverride is not null && serialPorts.Contains(serialPortOverride)
                ? serialPortOverride
                : serialPorts.LastOrDefault();

            if (string.IsNullOrWhiteSpace(chosenSerialPort))
                _logger.LogWarning("No serial port found");
            else
            {
                _logger.LogInformation("Chosen serial port: {SerialPort}", chosenSerialPort);
                _serial = new SerialPort(chosenSerialPort, bauldRateOverride ?? 9600);
            }
        }

        public override TimeSpan InitialDelay => TimeSpan.Zero;

        public override TimeSpan Interval => TimeSpan.FromSeconds(1);

        public override async Task Run()
        {
            if (_serial == null)
                await StopAsync(new CancellationToken());
            else
            {
                var state = await ReadState();
                if (Count % 10 == 0)
                    _logger.LogInformation(state.ToString());
                await SaveState(state);
            }
        }

        public async Task SaveState(NobreakState state)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<IDbContext>();
            var lastState = await context.NobreakStates.OrderBy(s => s.Timestamp).LastOrDefaultAsync();
            if (lastState == null || lastState.PowerState != state.PowerState)
                context.Add(new NobreakStateChange(state));
            context.Add(state);
            await context.SaveChangesAsync();
        }

        public async Task<NobreakState> ReadState()
        {
            OpenSerialPort();
            _serial.Write("Q1\r");
            var res = await Task.Run(() => _serial.ReadTo("\r"), new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
            return NobreakState.FromSerialResponse(res);
        }

        public void OpenSerialPort()
        {
            try
            {
                if (!_serial.IsOpen)
                    _serial.Open();
            }
            catch (Exception e)
            {
                throw new Exception($"Could not connect to port {_serial.PortName}: {e.Message}", e);
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            _serial?.Dispose();
        }
    }
}
