using MassTransit;
using RabbitMQ.ESB.MassTransit.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.ESB.MassTransit.Consumer.Consumer
{
    public class ExampleMessageConsumer : IConsumer<IMessage>
    {
        //ilgili kuyruğa Imessage turunden Mesaj gelirse bu consumer consume
        //func tetikliyecek
        public Task Consume(ConsumeContext<IMessage> context)
        {
            Console.WriteLine($" Gelen mesaj : {context.Message.Text}");
            return Task.CompletedTask;
        }
    }
}
