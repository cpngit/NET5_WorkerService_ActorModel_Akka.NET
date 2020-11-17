using System;
using Akka.Actor;
using Akka.DI.Core;
using NET5.ActorModel.Core;

namespace NET5.ActorModel.Actor
{
    public class NotificationActor : UntypedActor
    {
        private readonly ILoggerAdapter<NotificationActor> _logger;
        private readonly IEmailNotification emailNotification;
        private readonly IActorRef childActor;

        public NotificationActor(IEmailNotification emailNotification, ILoggerAdapter<NotificationActor> logger)
        {
            this.emailNotification = emailNotification;
            this._logger = logger;
            this.childActor = Context.ActorOf(Context.System.DI().Props<LogNotificationActor>());
        }

        protected override void OnReceive(object message)
        {
            _logger.LogInformation($"Inside of NotificationActor - Message received: {message}");
            emailNotification.Send(message?.ToString());
            childActor.Tell(message);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromMinutes(1),
                localOnlyDecider: ex =>
                {
                    return ex switch
                    {
                        ArgumentException ae => Directive.Resume,
                        NullReferenceException ne => Directive.Restart,
                        _ => Directive.Stop
                    };
                }
                );
        }

        protected override void PreStart() => _logger.LogInformation($"Inside of NotificationActor - Actor started.");

        protected override void PostStop() =>  _logger.LogInformation($"Inside of NotificationActor - Actor stopped.");
    }
}