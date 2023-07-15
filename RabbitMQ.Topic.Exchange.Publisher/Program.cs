using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new()
{
    HostName = "localhost"
};
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "topic-exchange-example",
    type: ExchangeType.Topic
    );

for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba {i}");
    Console.Write("Mesajın gönderileceği topic formatını belirtiniz : ");
    //ex 
    string topic = Console.ReadLine();
    channel.BasicPublish(
        exchange: "topic-exchange-example",
        routingKey: topic,
        body: message
        );
}

Console.Read();