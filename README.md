# RabbitMQ

Implementação de **Produtor** e **Consumidor** de Mensagens RabbitMQ em Filas.



## Conteúdo

### Projeto base
Projeto padrão com simples implementação de exemplo com Produtor e Consumidor de mensagens em filas do RabbitMQ.
Para os testes/implementações neste **projeto base**  foram dois servidores RabbitMQ distintos, são eles:

**Imagem Docker do RabbitMQ**
 a imagem do docker que foi baixada a partir da linha de comando a segui:

    docker run -d --hostname rabbitserver --name rabbitmq-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management

Para acessar o sistema admin do RabbitMQ localmente acessar a seguinte URL:
*http://localhost:8080/*

Usuário: guest
Senha: guest

**Servidor CloudAMQP** 
Foi criador um servidor CloudAMQP para que testes e implementações em um cenário diferente do primeiro fosse possível.


### Projeto avançado
Pretendo realizar implementações um pouco mais avançadas. Ainda não realizado...
