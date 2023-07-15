using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new()
{
    HostName = "localhost"
};
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

#region P2P (Point-to-Point) Tasarımı 
/*
 *Bu tasarımda bir publisher iligli mesaji direk bir kuruğa gönderir ve bu mesaj kuyrugu işleyen bir consumer tarafındn tüketilir
 *eğerki bir mesaj bir tüketici tarafından işlenmesi gerekiyor ise bu tasarım kullanılır 
 *direct exhange uygun
 */


//string queueName = "example-p2p-queue";

//channel.QueueDeclare(
//    queue: queueName,
//    durable: false,
//    exclusive: false,
//    autoDelete: false);

//byte[] message = Encoding.UTF8.GetBytes("merhaba");
//channel.BasicPublish(
//    exchange: string.Empty,
//    routingKey: queueName,
//    body: message);
#endregion
#region Publish/Subscribe (Pub/Sub) Tasarımı
/*
 * Bu tasarımda publisher mesajı bir exhange'e gönderir ve böylece mesaj bu exhange'e bind edilmiş olan tüm kuyruklara yönlendirilir 
 * bu tasarım bir mesajin birden çok tüketici tarafından işlenmesi gerekdiği durumda kullanılır
 * Fanout exchange kullanılır
 */


//string exchangeName = "example-pub-sub-exchange";

//channel.ExchangeDeclare(
//    exchange: exchangeName,
//    type: ExchangeType.Fanout);

//for (int i = 0; i < 100; i++)
//{
//    await Task.Delay(200);

//    byte[] message = Encoding.UTF8.GetBytes("merhaba" + i);

//    channel.BasicPublish(
//        exchange: exchangeName,
//        routingKey: string.Empty,
//        body: message);
//}

#endregion
#region Work Queue(İş Kuyruğu) Tasarımı​
/*
 * Bu tasarımda publisher tarafından yayınlanmış bir mesajın birden fazla consumer arasından yalnızca biri tarafından tüketilmesi amaçlanmaktadır.
 *  böylece mesajların işlenmesi sürecinde tüm consumer'lar aynı iş yüküne ve eşit görev dağılımınasahip olacaktır
 * 
 * direct exchnge
 */


//string queueName = "example-work-queue";

//channel.QueueDeclare(
//    queue: queueName,
//    durable: false,
//    exclusive: false,
//    autoDelete: false);

//IBasicProperties properties = channel.CreateBasicProperties();  
//  properties.Persistent = true;

//for (int i = 0; i < 100; i++)
//{
//    await Task.Delay(200);

//    byte[] message = Encoding.UTF8.GetBytes("merhaba" + i);

//    channel.BasicPublish(
//        exchange: string.Empty,
//        routingKey: queueName,
//        body: message,
//        basicProperties:properties);
//}

#endregion
#region Request/Response Tasarımı​
/*
 * Bu tasarımda publisher bir request yapar gibi kuyruğa mesj gönderir ve bu mesaji tüketen consumer'dan sonuca dair başka kuyruktam bir yanıt response bekler.
 * * 
 * 
 */

//Response'un dinleneceği  queue tanımlanyor
string requestQueueName = "example-request-response-queue";

//Response sürecinde hangi request'e karşılık response'un yapılacağını ifade edecek olan korelasyonel değer oluşturuluyor
channel.QueueDeclare(
    queue: requestQueueName,
    durable: false,
    exclusive: false,
    autoDelete: false);

string replyQueueName = channel.QueueDeclare().QueueName;

string correlationId = Guid.NewGuid().ToString();

#region Request Mesajını Oluşturma ve Gönderme
IBasicProperties properties = channel.CreateBasicProperties();
//Request korelasyon değeri ile eşleştiriliyor
properties.CorrelationId = correlationId;
//Response yapılacak queue RepleyTo property'sine atanıyor
properties.ReplyTo = replyQueueName;

for (int i = 0; i < 10; i++)
{
    byte[] message = Encoding.UTF8.GetBytes("merhaba" + i);
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: requestQueueName,
        body: message,
        basicProperties: properties);
}
#endregion
#region Response Kuyruğu Dinleme
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: replyQueueName,
    autoAck: true,
    consumer: consumer);

consumer.Received += (sender, e) =>
{
    if (e.BasicProperties.CorrelationId == correlationId)
    {
        //Recieve edilen mesajın request'te ki korelasyon değeriyle aynı olup olmadığı kontrol ediliyor ve eğer aynısıysa iligli mesaj response değeri olarak algılanıp işleme tabi tutuluyor
        Console.WriteLine($"Response : {Encoding.UTF8.GetString(e.Body.Span)}");
    }
};
#endregion

#endregion

Console.Read();