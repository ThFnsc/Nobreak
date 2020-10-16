using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Text.RegularExpressions;
using Nobreak.Models;
using Nobreak.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nobreak.Context.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading;

namespace Nobreak.Services.Serial
{
    public class NobreakSerialMonitor : TimedHostedService, IDisposable
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<NobreakSerialMonitor> _logger;
        private readonly SerialPort _serial;
        private readonly IServiceProvider _serviceProvider;

        public NobreakSerialMonitor(ILogger<NobreakSerialMonitor> logger, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings) : base(logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;

            var serialPorts = SerialPort.GetPortNames();
            _logger.LogInformation("Portas seriais encontradas: {SerialPorts}", string.Join(", ", serialPorts));
            var chosenSerialPort = _appSettings.SerialPort ?? serialPorts.Last();
            if (string.IsNullOrWhiteSpace(chosenSerialPort))
                _logger.LogWarning("Nenhuma porta serial escolhida/encontrada");
            else
            {
                _logger.LogInformation("Porta serial escolhida: {SerialPort}", chosenSerialPort);
                _serial = new SerialPort(chosenSerialPort, _appSettings.BauldRate ?? 9600);
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
            using var context = scope.ServiceProvider.GetService<NobreakContext>();
            var lastState = await context.NobreakStates.OrderBy(s => s.Timestamp).LastOrDefaultAsync();
            if (lastState == null || lastState.PowerState != state.PowerState)
                context.NobreakStateChanges.Add(new NobreakStateChange { NobreakState = state });
            context.NobreakStates.Add(state);
            await context.SaveChangesAsync(true);
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
                throw new Exception($"Não foi possível conectar na porta {_serial.PortName}: {e.Message}", e);
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            _serial?.Dispose();
        }
    }
}
