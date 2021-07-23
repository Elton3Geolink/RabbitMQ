using Rabbit.Comum;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.Producer
{
    class Program
    {
     

        static void Main(string[] args)
        {
            //Comando para rodar o RabbitMQ em container (utilizado nesta POC):
            // docker run -d --hostname rabbitserver --name rabbitmq-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management


            string fila = "FILA_TESTE";

            RabbitFila rabbitFila = new RabbitFila(TipoAcessoConexao.CloudAMQP);

            ConnectionFactory factory = rabbitFila.ObterConexao();

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: fila,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


                string message = "";
                int cont = 0;

                while (true)
                {
                    message = $"Mensagem - {cont}";
                    cont++;

                    Console.WriteLine($"MSG Enviada: {message}");

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: fila,
                                         basicProperties: null,
                                         body: body);

                    Thread.Sleep(200);
                }
            }
        }
    }
}
