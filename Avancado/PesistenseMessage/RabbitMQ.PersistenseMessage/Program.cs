using Rabbit.Comum;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.PersistenseMessage
{
    class Program
    {


        static void Main(string[] args)
        {
            //Comando para rodar o RabbitMQ em container (utilizado nesta POC):
            // docker run -d --hostname rabbitserver --name rabbitmq-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management


            string fila = "FILA_TESTE_DURAVEL";

            RabbitFila rabbitFila = new RabbitFila(TipoAcessoConexao.Local);

            ConnectionFactory factory = rabbitFila.ObterConexao();

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: fila,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var basicProp = channel.CreateBasicProperties();
                basicProp.Persistent = true;

                string message = $"Mensagem - Teste";                
                
                Console.WriteLine($"MSG Enviada: {message}");

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                        routingKey: fila,
                                        basicProperties: basicProp,
                                        body: body);


                Console.WriteLine("Pressione qualquer tecla para finalizar");
                Console.ReadKey();
            }
        }
    }
}
