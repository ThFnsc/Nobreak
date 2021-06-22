using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Nobreak.Helpers
{
    public class LogCircuits : CircuitHandler
    {
        private int _count;
        private readonly object _lock = new();
        private readonly ILogger<LogCircuits> _logger;

        public LogCircuits(ILogger<LogCircuits> logger)
        {
            _logger = logger;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            lock (_lock)
                _logger.LogTrace("Circuit opened. Now {TotalCircuits} in total.", ++_count);
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            lock (_lock)
                _logger.LogTrace("Circuit closed. Now {TotalCircuits} in total.", --_count);
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }
}
