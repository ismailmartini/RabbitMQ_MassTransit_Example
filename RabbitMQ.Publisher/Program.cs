using RabbitMQ.Client;
using System.Text;

//Bağlantı Oluşturma cloud
//ConnectionFactory factory = new();
//factory.Uri = new("amqps://befjdvjy:iVeqbKaYkGVShpfp4SdeT7iNbPiuVOD2@moose.rmq.cloudamqp.com/befjdvjy");


//Bağlantı Oluşturma

ConnectionFactory factory = new() { HostName = "localhost" };


//Bağlantıyı Aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


//Queue Oluşturma
channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true);

/*
 * Message Durability Rabbitmq sunucsunun kapanma vs durumlarında mesajların kaybolmaması için 
 * durable:true bu değeri true olarak veriyoruz (kuruk için konfigurasyon) consumer tarafındada aynı parametre verilmeli
 * 2 adım
  IBasicProperties properties = channel.CreateBasicProperties(); (bu mesaj için konfugrasyon)
  properties.Persistent = true;
 * %100 kalıcı olacağınzı garnati etmek outbox inbox design pattern kullanılması gerek
 */

/*
 * Fair Dispatch RabbitMQ'da tüm consumer'lara eşit şekilde mesaj iletilebilir
 * kuyruktaki tüm mesajları adil şekilde consumer'lara dağıtır performan için olumlu 
 * consumer'da basicQos parametresini yapılandırıyoruz
 * 
 * */





//Queue'ya Mesaj Gönderme

//RabbitMQ kuyruğa atacağı mesajları byte türünden kabul etmektedir. Haliyle mesajları bizim byte dönüşmemiz gerekecektir.
//byte[] message = Encoding.UTF8.GetBytes("Merhaba");
//channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message);

IBasicProperties properties = channel.CreateBasicProperties();
properties.Persistent = true;

for (int i = 0; i < 50; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes("Merhaba " + i);
    channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message, basicProperties: properties);
}

Console.Read();