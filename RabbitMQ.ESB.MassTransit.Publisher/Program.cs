using MassTransit;
using RabbitMQ.ESB.MassTransit.Shared.Messages;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

//cloud

//string rabbitMQUriCloud = "amqps://befjdvjy:bs5zD-4j8OfHQrZFUOnEAKomCudYmkL1@moose.rmq.cloudamqp.com/befjdvjy";

//IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
//{
//     factory.Host(rabbitMQUri);

//});
string queueName = "example-queue";
string rabbitMQUri = "rabbitmq://localhost";
IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
});


//publishden farklı send direk kuyruga gnderir"

//send tekbir kuyruga tek bir noktya mesaj gönderirken kullanıyoruz
ISendEndpoint sendEndpoint = await bus.GetSendEndpoint(new($"{rabbitMQUri}/{queueName}"));

Console.Write("Gönderilecek mesaj : ");
string message = Console.ReadLine();
//shared'daki ortak mesaj implementasyonu
await sendEndpoint.Send<IMessage>(new ExampleMessage()
{
    Text = message
});

Console.Read();