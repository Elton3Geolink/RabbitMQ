using Rabbit.Comum;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.PublishConfirmation
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

                //Adicao de publish confirmation
                channel.ConfirmSelect();

                //Adicao de evendo ACK e NACK para receber o publish confirmation
                channel.BasicAcks += Channel_BasicAcks;
                channel.BasicNacks += Channel_BasicNacks;

                //Adicao de Evento para verificar se o mensagem foi enviada para uma fila existente
                channel.BasicReturn += Channel_BasicReturn;


                channel.QueueDeclare(queue: fila,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


                string message = $"Mensagem - PUBLISH CONFIRMATION";                
                
              
                Console.WriteLine($"MSG Enviada: {message} - Data: {DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}");

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "",//Vai para exchange default
                    routingKey: fila,
                    basicProperties: null,
                    body: body,
                    mandatory:true
                    );
                


                //////ENVIANDO PARA UM EXCHANGE INEXISTENTE
                ////channel.BasicPublish(
                ////  exchange: "exchange_inexistente",//Vai para que nao existe: necessário adicionar evento "WatForConfirms" que dispara exception por nao existir a exchange
                ////                                   //OBS.: Neste caso de exchange inexistente o evento de NACK não é acionado. Por isto deve acionar o evento "WatForConfirms"
                ////  routingKey: fila,
                ////  basicProperties: null,
                ////  body: body
                ////  );

                //channel.WaitForConfirms(new TimeSpan(0, 0, 5));


                ////ENVIANDO PARA UMA FILA INEXISTENTE
                ////Vai cair no evendo ACK caso o mandatory nao seja configurado. Por isto é necessário ativar a flag "Mandatory" como true e adicionar mais um evento "escutar": BasicReturn
                //channel.BasicPublish(
                //  exchange: "",
                //  routingKey: fila + "_FILA_INEXISTENTE",
                //  basicProperties: null,
                //  body: body,
                //  mandatory: true //Flag necessaria para verificar se a mensagem foi enviada para uma fila existente.
                //  );

                //channel.WaitForConfirms(new TimeSpan(0, 0, 5));

            }
        }

        private static void Channel_BasicNacks(object sender, BasicNackEventArgs e)
        {
            Console.WriteLine("EVENTO Channel_BasicNacks");
        }
           
        private static void Channel_BasicAcks(object sender, BasicAckEventArgs e)
        {
            Console.WriteLine("EVENTO Channel_BasicAcks");
        }


        /// <summary>
        /// É necessário setar a flag "Mandatory" do channel como TRUE para que este evento seja acionado.
        /// E somente será acionado em caso de erro como fila inexistente por exemplo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Channel_BasicReturn(object sender, BasicReturnEventArgs e)
        {            

            Console.WriteLine($"EVENTO Channel_BasicReturn: Mensagem enviada: {Encoding.UTF8.GetString(e.Body.ToArray())}");
        }
    }
}
