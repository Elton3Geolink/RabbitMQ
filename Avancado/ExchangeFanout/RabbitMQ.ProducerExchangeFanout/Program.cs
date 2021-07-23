using Rabbit.Comum;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.ProducerExchangeFanout
{
    class Program
    {
        static void Main(string[] args)
        {
            //Comando para rodar o RabbitMQ em container (utilizado nesta POC):
            // docker run -d --hostname rabbitserver --name rabbitmq-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management

            RabbitFila rabbitFila = new RabbitFila(TipoAcessoConexao.Local);

            ConnectionFactory factory = rabbitFila.ObterConexao();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            using (var connection = factory.CreateConnection())
            {
                string queueName = "TESTE_FILA";

                var channel = SetupChannel(connection);

                BuildAndRunPublishers(channel, queueName, "PRODUTOR_XPTO", manualResetEvent);

                manualResetEvent.WaitOne();

            }
        }


        static IModel SetupChannel(IConnection connection)
        {
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "FILA_TESTE", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "FILA_2", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "FILA_3", durable: false, exclusive: false, autoDelete: false, arguments: null);

            channel.ExchangeDeclare("teste_fanout", type: "fanout");            

            channel.QueueBind("FILA_TESTE", exchange: "teste_fanout",routingKey:"");
            channel.QueueBind("FILA_2", exchange: "teste_fanout", routingKey: "");
            channel.QueueBind("FILA_3", exchange: "teste_fanout", routingKey: "");            


            //Fazendo o bind (associação) entre o exchange e a fila, ou seja, informo qual exchange se "ligará" a certa fila.

            return channel;
        }

        static void BuildAndRunPublishers(IModel channel, string queue, string publisherName, ManualResetEvent manualResetEvent)
        {
            Task.Run(() => {

                int count = 0;

                while (true)
                {
                    try
                    {
                        Console.WriteLine("Pressione qualquer tecla para produzir 10 mensagens.");
                        Console.ReadLine();

                        for(int i = 0; i < 10; i++)
                        {
                            string mensagem = $"Mensagem {i}";
                            byte[] body = Encoding.UTF8.GetBytes(mensagem);

                            channel.BasicPublish(exchange: "teste_fanout", routingKey: queue, basicProperties: null, body);
                        }

                    }
                    catch(Exception ex)
                    {
                        //Log...
                        Console.WriteLine($"Exception:{ex.Message}");
                    }
                }
            });
        }
    }
}
