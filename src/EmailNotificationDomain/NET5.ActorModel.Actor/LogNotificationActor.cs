using System;
using Akka.Actor;
using NET5.ActorModel.Core;

namespace NET5.ActorModel.Actor
{
    public class LogNotificationActor : UntypedActor
    {
        private readonly ILoggerAdapter<LogNotificationActor> _logger;

        public LogNotificationActor(ILoggerAdapter<LogNotificationActor> loggerAdapter)
        {
            this._logger = loggerAdapter;
        }


        protected override void PreStart() => 
            _logger.LogInformation("LogNotificationActor child stared!...");

        protected override void PostStop() =>
             _logger.LogInformation("LogNotificationActor child stopped!...");

        protected override void OnReceive(object message)
        {
            if (message.ToString() == "n")
                throw new NullReferenceException();
            if (message.ToString() == "e")
                throw new ArgumentException();
            if (string.IsNullOrEmpty(message.ToString()))
                throw new Exception();

            _logger.LogInformation($"Inside Of ChildActor LogNotificationActor - Sending text message {message}...");
        }
    }
}