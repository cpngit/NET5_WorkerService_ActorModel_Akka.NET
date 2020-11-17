using System;
using NET5.ActorModel.Core;

namespace NET5.ActorModel.Infrastructure
{
    public class EmailNotification : IEmailNotification
    {
        public void Send(string message)
        {
            Console.WriteLine($"Sending email with message: {message}");
        }
    }
}