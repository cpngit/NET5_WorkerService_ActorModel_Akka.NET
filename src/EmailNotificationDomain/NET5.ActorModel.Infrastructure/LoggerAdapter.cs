using Microsoft.Extensions.Logging;
using NET5.ActorModel.Core;
using System;

namespace NET5.ActorModel.Infrastructure
{
    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;
    
        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }
    
        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }
    
        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }
    }
}