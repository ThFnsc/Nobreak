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

namespace Nobreak.Services.Serial
{
    public class NobreakSerialMonitor : TimedHostedService, IDisposable
    {
        private readonly ILogger<NobreakSerialMonitor> _logger;
        
        private readonly SerialPort _serial;
        
        private readonly IServiceProvider _serviceProvider;

        public NobreakSerialMonitor(ILogger<NobreakSerialMonitor> logger, IServiceProvider serviceProvider, IConfiguration configuration) : base(logger)
        {
            _logger = logger;
            _serial = new SerialPort(string.IsNullOrWhiteSpace(configuration["Variables:SerialPort"]) 
                ? SerialPort.GetPortNames().Last() 
                : configuration["Variables:SerialPort"], 9600);
            _serviceProvider = serviceProvider;
        }

        public override TimeSpan InitialDelay => TimeSpan.Zero;

        public override TimeSpan Interval => TimeSpan.FromSeconds(1);

        public override async Task Run()
        {
            await OpenSerialPort();
            _serial.Write("Q1\r");
            var res = _serial.ReadTo("\r");
            var state = NobreakState.FromSerialResponse(res);
            if(Count%10==0)
                _logger.LogInformation(state.ToString());
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<NobreakContext>();
            var lastState = await context.NobreakStates.OrderBy(s=>s.Timestamp).LastOrDefaultAsync();
            if (lastState == null || lastState.PowerState != state.PowerState)
                context.NobreakStateChanges.Add(new NobreakStateChange { NobreakState = state });
            context.NobreakStates.Add(state);
            await context.SaveChangesAsync(true);
        }

        public Task OpenSerialPort() =>
            Task.Run(() =>
            {
                if (!_serial.IsOpen)
                    _serial.Open();
            });

        public new void Dispose()
        {
            base.Dispose();
            _serial?.Dispose();
        }
    }
}
