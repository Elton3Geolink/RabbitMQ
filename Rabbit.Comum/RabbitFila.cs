using RabbitMQ.Client;
using System;

namespace Rabbit.Comum
{

    public enum TipoAcessoConexao
    {
        Local = 1,
        CloudAMQP = 2
    }

    public class RabbitFila
    {
        private readonly TipoAcessoConexao _tipoAcessoConexao;
        public RabbitFila(TipoAcessoConexao tipoAcessoConexao)
        {
            _tipoAcessoConexao = tipoAcessoConexao;
        }


        public ConnectionFactory ObterConexao()
        {
            switch (_tipoAcessoConexao)
            {
                case TipoAcessoConexao.Local:

                    return new ConnectionFactory()
                    {
                        HostName = "localhost",
                        UserName = ConnectionFactory.DefaultUser,
                        Password = ConnectionFactory.DefaultPass,
                        Port = AmqpTcpEndpoint.UseDefaultPort
                    };

                case TipoAcessoConexao.CloudAMQP:

                    return new ConnectionFactory
                    {
                        Uri = new Uri("amqps://szkhjeke:37ul8zvLXP5fut_e5AoJGCc5tg8-Rv-q@elk.rmq2.cloudamqp.com/szkhjeke")
                    };


                default:
                    throw new Exception("Passe o tipo de conexao correto");
            }
        }

    }
}
