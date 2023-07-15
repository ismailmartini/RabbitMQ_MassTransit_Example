using MassTransit;
using RabbitMQ.ESB.MassTransit.Consumer.Consumer;


//cloud

//string rabbitMQUriCloud = "amqps://befjdvjy:bs5zD-4j8OfHQrZFUOnEAKomCudYmkL1@moose.rmq.cloudamqp.com/befjdvjy";

//IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
//{
//     factory.Host(rabbitMQUri);

//});

//consumer'lar türlere özel çalışıyor
//consumerların hangi türde çalışacağınız impelmentasyondaki mesaj türleri belirler
string queueName = "example-queue";
string rabbitMQUri = "rabbitmq://localhost";
IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);

    //consumer tanımlıyoruz
    factory.ReceiveEndpoint(queueName, endpoint =>
    {
        endpoint.Consumer<ExampleMessageConsumer>();
    });
});
await bus.StartAsync();

Console.Read();