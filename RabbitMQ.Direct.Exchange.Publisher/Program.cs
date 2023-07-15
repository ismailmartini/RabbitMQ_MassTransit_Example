 using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new()
{
	HostName = "localhost"
};

 
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

while (true)
{
	Console.WriteLine("add Message : ");
	string message = Console.ReadLine();
	byte[] byteMessage=Encoding.UTF8.GetBytes(message);
	channel.BasicPublish(
		exchange: "direct-exchange-example",
		routingKey:"direct-queue-example",
		body:byteMessage);
}

Console.Read();