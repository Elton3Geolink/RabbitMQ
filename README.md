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




### Projetos avançados
**Multiplos consumidores**

Redimensionamento simples de mensageria
----------------------------------------

Cenário:

- Produtor enviando mensagens e consumidor não tem capacidade de consumí-las a tempo hábil.
Sendo assim a fila vai ficando cada vez mais cheia por não haver vazão.

- Solução:
Criar multiplos consumidores para processarem as mensagens.
	-Vale lembrar quem não há problema de cada consumidor processar a mesma mensagem visto que 
	o RabbitMQ utiliza o algorítimo "Round Robbin" entregando somente mensagens distintas entre os consumidores.


- Executar projetos:
	Execute o projeto RabbitMQ.Producer junto com o projeto RabbitMQ.ConsumerMultiplo.

- Simulações
	- Para simular o Produtor envia uma 5 mensagens por segundo enquanto o consumidor consome 1 mensagem por segundo.
	Para isto utilizei sleep de 2 segundos na leitura da mensagem do consumidor. Sendo assim, para dar vazão nas mensagens,
	serão adicionados 6 consumidores para processar a mensagem. Com este dimensionamento a capacidade de processamento será
	superior à capacidade de emissão de mensagens.


## Replicar mensagens para mais de uma fila
### Exchange Fanout
Para realizar a publicação de uma mesma mensagem para mais de uma fila simultaneamente a opção indicada na documentação do RabbitMQ é a utilização da Exchange **fanout**.
Foi adicionado à solution na pasta "Avancado\ExchangeFanout" um novo projeto chamado "RabbitMQ.ProducerExchangeFanout" onde foi feita a implementação desta funcionalidade.
Se configurado o servidor RabbitMQ seja em docker ou CloudAMQP, basta rodar este projeto isoladamente e verificar o comportamento no AdminUI do RabbitMQ.


### Exchange Direct
Dependento da regra de negócio do sistema pode ser necessário redirecionar mensagem para devidas filas de acordo com o roteamento (routing). Por exemplo:
 
 - Quando uma mensagem se tratar de uma inserção esta deverá ser enviada para as filas de A e B.
 - Quando a mensagem se tratar de uma atualização ela deverá ser enviada para a fila B somente.

Foi criado um novo projeto chamado **RabbitMQ.ProducerExchangeDirect** que está dentro da pasta **..\Avancado\ExchangeDirect**

Se configurado o servidor RabbitMQ seja em docker ou CloudAMQP, basta rodar este projeto isoladamente e verificar o comportamento no AdminUI do RabbitMQ.


### Persistence Message | Queue Durable

Trata-se de manter persistencia das mensagens publicadas e também as filas no RabbitMQ para casos onde 
o servidor caia, ao subir novamente mantenha as filas e suas mensagens.

Foi criado um novo projeto chamado **RabbitMQ.ProducerExchangeDirect** que está dentro da pasta **..\Avancado\PesistenseMessage**
