using MassTransit;
using RabbitMQ.ESB.MassTransit.Shared.RequestResponseMessages;

Console.WriteLine("Publisher");

//string rabbitMQUri = "amqps://befjdvjy:bs5zD-4j8OfHQrZFUOnEAKomCudYmkL1@moose.rmq.cloudamqp.com/befjdvjy";
string rabbitMQUri = "rabbitmq://localhost";

string requestQueue = "request-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
});

//publisher startlıyoruz publish edilen mesajı consumer gibi okuyacak
await bus.StartAsync();

var request = bus.CreateRequestClient<RequestMessage>(new Uri($"{rabbitMQUri}/{requestQueue}"));

int i = 1;
while (true)
{
    await Task.Delay(200);
    //gönderdiğimiz mesaj içindeki mesaj no muzu bekliyecek şekilde mesajımı yolluyoruz
    var response = await request.GetResponse<ResponseMessage>(new() { MessageNo = i, Text = $"{i++}. request" });
    Console.WriteLine($"Response Received : {response.Message.Text}");
}

Console.Read();