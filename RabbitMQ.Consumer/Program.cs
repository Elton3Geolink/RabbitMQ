using Rabbit.Comum;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Comando para rodar o RabbitMQ em container (utilizado nesta POC):
            // docker run -d --hostname rabbitserver --name rabbitmq-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management

            string fila = "FILA_TESTE";

            RabbitFila rabbitFila = new RabbitFila(TipoAcessoConexao.Local);

            ConnectionFactory factory = rabbitFila.ObterConexao();

         
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: fila,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        
                        Console.WriteLine($"MSG Recebida: {message}");

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch(Exception ex)
                    {

                        //Logar erro

                        //Nack na mensagem
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }

       
                };
                channel.BasicConsume(queue: fila,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
