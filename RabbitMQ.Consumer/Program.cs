using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Bağlantı Oluşturma 
ConnectionFactory factory = new() { HostName = "localhost" };
 

//Bağlantı Aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

//Queue Oluşturma
channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true);//Consumer'da da kuyruk publisher'daki ile birebir aynı yapılandırmada tanımlanmalıdır!

//Queue'dan Mesaj Okuma
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: "example-queue", autoAck: false, consumer);

/*
 * autoAck =false Message Acknowledgement aktifleştirioruz - mesaj onaylama süreci aktifleşiyor (mesajın kuyruktan silinmesi
 * Message Acknowledgement'da cunnsomer'dan gönderilecek bildiriye kalıyor)
 * 
 * autoAck=true ise tüketilen mesajlar sonuca bakılmaksızın queue 'dan silinir
 */


//Fair Dispatch konfigrasyonu  consumer'lara eşit şekilde mesaj tüketebilirliği
// BasicQos(0, 1, false);
// prefetchSize:0 => bir consumer tarafından alınabilecek en büyük mesaj boyutu byte cinsinden belirtiler 0 sınırısız demek
// prefetchCount:1=> bir consumer tarafından aynı anda işleme alınabilecek mesaj sayısını belirtir
// global:false=> bu konf. tüm consumer'lar içinmi yoksa sadece çağrıyı yapan consumer için mi geçerli olacağını belirtir
channel.BasicQos(0, 1, false);
consumer.Received += (sender, e) =>
{
    //Kuyruğa gelen mesajın işlendiği yerdir!
    //e.Body : Kuyruktaki mesajın verisini bütünsel olarak getirecektir.
    //e.Body.Span veya e.Body.ToArray() : Kuyrukdaki mesajın byte verisini getirecektir.
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));

    channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
    /*
     *autoAck =false consemer mesajı başarılı şekilde işlediği bilgisini gönderiyor ve mesaj kuyruktan siliniyor
     *multiple birden fazla mesaja dair onay bildirisi gönderir. Eğer true ise deliverytag değerine sahip olan bu mesajla birlikte
     *bundan önceki mesajlarında  hepsininde işlendiğini onaylar.
     *multiple false sadece bu mesaj için onay bildirisinde bulunur
     *
     *
     * - BasicNack - bazı durumlarda mesajı işlemek istemeye biliriz bu seneryodada 
     
      channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false,requeue:true);

    * channel.BasicNack metodu kullnarak RabbitMq'ya bilgi verebilir e mesajı tekrardan işletebiliriz
    * requeue :true mesaj kuyruğa yeniden işlenmesi için geri eklenecek
    * requeue : false ise bu mesajın kuyruğa eklenmeden silinmesini sağlayacak isteğimiz izle mesajı çöpe atmış ve bu durumdada RabbitMq'Ya bilgi vermiş olacaktır
    * 
    * - BasicCancel - bir kuyruktaki tüm mesajların işlenmesini reddetme
    
     var consumerTag=channel.BasicCancel(deliveryTag: e.DeliveryTag, multiple: false,autoAck:false,consumer:consumer);
    * channel.BasicCancel(consumerTag)
    * bütün kuyruktaki tüm mesajlar red edilerek işlenmez
    * 
    * 
    *  - BasicReject - tek bir mesajın işlenmesini reddetme
        channel.BasicReject(deliveryTag:3,requeue:true)
    *  
    */

};

Console.Read();