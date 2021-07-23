using Rabbit.Comum;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.ProducerExchangeDirect
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
                var channel = SetupChannel(connection);

                BuildAndRunPublishers(channel, "PRODUTOR_XPTO", manualResetEvent);

                manualResetEvent.WaitOne();
            }
        }


        static IModel SetupChannel(IConnection connection)
        {
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "FILA_INSERT_UPDATE", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "FILA_INSERT_UPDATE", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: "FILA_UPDATE", durable: false, exclusive: false, autoDelete: false, arguments: null);

            channel.ExchangeDeclare("teste_direct", type: "direct");


            //Comportamento esperado neste simulado:
            //Quando houver inserção de um registro (insert_xpto) a mensagem será enviada para as filas "FILA_INSERT_UPDATE" e "FILA_UPDATE"
            //Quando hovuer atualização (update_xpto) a mensagem será enviada somente para a fila "FILA_UPDATE"

            //Fazendo o bind (associação) entre o exchange e a fila, ou seja, informo qual exchange se "ligará" a certa fila.

            channel.QueueBind("FILA_INSERT_UPDATE", exchange: "teste_direct", routingKey: "insert_xpto");
            channel.QueueBind("FILA_INSERT_UPDATE", exchange: "teste_direct", routingKey: "update_xpto");
            channel.QueueBind("FILA_UPDATE", exchange: "teste_direct", routingKey: "update_xpto");

            return channel;
        }

        static void BuildAndRunPublishers(IModel channel, string publisherName, ManualResetEvent manualResetEvent)
        {
            Task.Run(() =>
            {

                while (true)
                {
                    try
                    {
                        Console.WriteLine("Pressione qualquer tecla para produzir a mensagem.");
                        Console.ReadLine();

                        //MENSAGEM DIRETA PARA FILA DE INSERT (INSERT_XPTO)
                        string mensagem = $"Msg criada - from Publisher {publisherName} - Data: {DateTime.Now.ToString("dd/MM/yy HH:mm:ss")} - {Guid.NewGuid()}";
                        byte[] body = Encoding.UTF8.GetBytes(mensagem);

                        Console.WriteLine($"INSERT_XPTO - {mensagem}");
                        channel.BasicPublish(exchange: "teste_direct", routingKey: "insert_xpto", basicProperties: null, body);



                        //MENSAGEM DIRETA PARA FILA DE UPDATE (UPDATE_XPTO)
                        string mensagem2 = $"Msg criada - from Publisher {publisherName} - Data: {DateTime.Now.ToString("dd/MM/yy HH:mm:ss")} - {Guid.NewGuid()}";
                        byte[] body2 = Encoding.UTF8.GetBytes(mensagem2);
                        channel.BasicPublish(exchange: "teste_direct", routingKey: "update_xpto", basicProperties: null, body2);
                    }
                    catch (Exception ex)
                    {
                        //Log...
                        Console.WriteLine($"Exception:{ex.Message}");

                        manualResetEvent.Set();
                    }
                }
            });
        }
    }
}
