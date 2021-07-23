using Rabbit.Comum;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.ConsumerMultiplo
{
    class Program
    {
        static string fila = "FILA_TESTE";

        static void Main(string[] args)
        {
            //Comando para rodar o RabbitMQ em container (utilizado nesta POC):
            // docker run -d --hostname rabbitserver --name rabbitmq-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management

            RabbitFila rabbitFila = new RabbitFila(TipoAcessoConexao.Local);

            ConnectionFactory factory = rabbitFila.ObterConexao();

            using (var connection = factory.CreateConnection())            
            {
                //Quanto mais consumidores forem criados maior será a vazão das mensagens lidas
                //Deve-se analizar a quantidade de consumidores a serem utilizados de acordo com 
                //a quantidade de mensagens enviadas para fila pelo(s) Producer(s) e quantidade de.
                //Acredito que sempre será necessário ter maior quantidade de vazão de mensagens para
                //que as mensagens não fiquem represadas no servidor RabbitMQ.

                int quantidadeConsumidoresCriar = 6;
             
                for(int i = 0; i < quantidadeConsumidoresCriar; i++)
                {
                    var channel = CreateChannel(connection);

                    channel.QueueDeclare(queue: fila,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    BuildAnRunWorker(channel, $"Consumidor {i}");                    
                }


                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        static IModel CreateChannel(IConnection connection)
        {
            var channel = connection.CreateModel();

            ushort qtdeMensagensLidasEmSequenciaPorConsumidor = 2;
            //Definir de quantas em quantas mensagens serão distribuídas pelo RabbitMQ através
            //do algorítimo RoundRobin.
            //Caso nao seja configurado (não é obrigatório utilizar a linha de configuração abaixo)
            // e já existam mensagens a serem lidas quando o sistema de consumidores "subir".
            // o primeiro consumidor fará o processamento de todas as mensagens existente no servidor RabbitMQ
            // e, somente quando todas as mensagens forem lidas, somente a partir das novas mensagens que chegarem 
            // no servidor RabbitMQ serão "distribuídas" aos múltiplos consumidores.
            // Obs.: Isto se trata de distriubição de carga de processamento entre consumidores. Pois 
            // cada consumidor pode estar recebendo uma mensagem diferente e, assim, processar um fluxo
            // diferente. Fluxos estes que podem ter processamentos mais rápidos ou não entre si.
            

            channel.BasicQos(0, qtdeMensagensLidasEmSequenciaPorConsumidor, false);

            return channel;



        }

        static void BuildAnRunWorker(IModel channel, string nomeConsumidor)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"{nomeConsumidor} - MSG Recebida: {message}");

                    channel.BasicAck(ea.DeliveryTag, false);

                    //Forçando a demora no consumo para simular gargalo
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {

                    //Logar erro

                    //Nack na mensagem
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }


            };
            channel.BasicConsume(queue: fila,
                                 autoAck: false,
                                 consumer: consumer);
        }
    }
}
