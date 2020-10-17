using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nobreak.Infra.Services
{
    public abstract class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private Task _lastRun;
        private readonly ILogger _logger;

        public int Count { get; set; } = 0;

        public abstract TimeSpan InitialDelay { get; }

        public abstract TimeSpan Interval { get; }

        public TimedHostedService(ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task Run();

        public void Dispose() =>
            _timer?.Dispose();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Dispose();
            _timer = new Timer(state =>
            {
                try
                {
                    if (_lastRun == null || _lastRun.Status != TaskStatus.Running && _lastRun.Status != TaskStatus.WaitingForActivation)
                    {
                        _lastRun = Run();
                        _lastRun?.Wait();
                        Count++;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error in service {GetType().Name}");
                }
            }, null, InitialDelay, Interval);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
