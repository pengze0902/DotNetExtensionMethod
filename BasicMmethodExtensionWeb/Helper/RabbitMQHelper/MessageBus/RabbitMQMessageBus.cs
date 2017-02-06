using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BasicMmethodExtensionWeb.Helper.RabbitMQHelper.Extentions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BasicMmethodExtensionWeb.Helper.RabbitMQHelper.MessageBus
{
    /// <summary>
    /// RabbitMQ 总线
    /// </summary>
    public class RabbitMQMessageBus : MessageBusBase
    {
        private readonly string _queueName;
        private readonly string _routingKey;
        private readonly string _exchangeName;
        private readonly bool _durable;
        private readonly bool _persistent;
        private readonly bool _exclusive;
        private readonly bool _autoDelete;
        private readonly string _hostName;
        private readonly bool _noAck = false;
        private readonly IDictionary<string,object> _queueArguments; 
        private readonly ConnectionFactory _factory;
        private readonly IConnection _publisherClient;
        private readonly IConnection _subscriberClient;
        private readonly IModel _publisherChannel;
        private readonly IModel _subscriberChannel;
        private readonly TimeSpan _defaultMessageTimeToLive = TimeSpan.MaxValue;

        public RabbitMQMessageBus(string hostNmae, string userName, string password, string queueName, string routingKey, string exhangeName, bool durable, bool persistent, bool exclusive, bool autoDelete,  IDictionary<string, object> queueArguments = null, TimeSpan? defaultMessageTimeToLive = null, bool noAck = false)
        {

            _hostName = hostNmae;
            _exchangeName = exhangeName;
            _queueName = queueName;
            _routingKey = routingKey;
            _durable = durable;
            _persistent = persistent;
            _exclusive = exclusive;
            _autoDelete = autoDelete;
            _queueArguments = queueArguments;

            if (defaultMessageTimeToLive.HasValue && defaultMessageTimeToLive.Value > TimeSpan.Zero)
                _defaultMessageTimeToLive = defaultMessageTimeToLive.Value;
            if(noAck!=null)
            {
                _noAck = noAck;
            }
            // initialize connection factory
            _factory = new ConnectionFactory
            {
                UserName = userName,
                Password = password,
                HostName = _hostName
                
            };

            // initialize publisher
            _publisherClient = CreateConnection();
            _publisherChannel = _publisherClient.CreateModel();
            SetUpExchangeAndQueuesForRouting(_publisherChannel);

            // initialize subscriber
            _subscriberClient = CreateConnection();
            _subscriberChannel = _subscriberClient.CreateModel();
            SetUpExchangeAndQueuesForRouting(_subscriberChannel);
        }

        /// <summary>
        /// The client sends a message to an exchange and attaches a routing key to it. 
        /// The message is sent to all queues with the matching routing key. Each queue has a
        /// receiver attached which will process the message. We’ll initiate a dedicated message
        /// exchange and not use the default one. Note that a queue can be dedicated to one or more routing keys.
        /// </summary>
        /// <param name="model">channel</param>
        private void SetUpExchangeAndQueuesForRouting(IModel model)
        {
            //http://melin.iteye.com/blog/691265
           //1.Direct Exchange – 处理路由键:
            //需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “dog”，则只有被标记为“dog”的消息才被转发，不会转发dog.puppy，也不会转发dog.guard，只会转发dog
            //2.Fanout Exchange – 不处理路由键:
            //你只需要简单的将队列绑定到交换机上。一个发送到交换机的消息都会被转发到与该交换机绑定的所有队列上。很像子网广播，每台子网内的主机都获得了一份复制的消息。Fanout交换机转发消息是最快的。 
            //3.Topic Exchange – 将路由键和某模式进行匹配:
            //此时队列需要绑定要一个模式上。符号“#”匹配一个或多个词，符号“*”匹配不多不少一个词。因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*” 只会匹配到“audit.irs”。我在RedHat的朋友做了一张不错的图，来表明topic交换机是如何工作的： 
            /*
             ExchangeName是该Exchange的名字，该属性在创建Binding和生产者通过publish推送消息时需要指定
             * ExchangeType，指Exchange的类型，在RabbitMQ中，有三种类型的Exchange：direct ，fanout和topic，不同的Exchange会表现出不同路由行为
             * Durable是该Exchange的持久化属性
             */
            model.ExchangeDeclare(_exchangeName, ExchangeType.Direct, _durable);
            // setup the queue where the messages will reside - it requires the queue name and durability.
            
            //队列声明 queuename 队列名称 durable=true：说明要持久化 exclusive=true :说明当Consumer关闭连接时，这个queue要被deleted  _autoDelete：是否自动删除队列中的消息
            /*
             Exclusive：排他队列，如果一个队列被声明为排他队列，该队列仅对首次声明它的连接可见，并在连接断开时自动删除。
             * 这里需要注意三点：其一，排他队列是基于连接可见的，同一连接的不同信道是可以同时访问同一个连接创建的排他队列的。
             * 其二，“首次”，如果一个连接已经声明了一个排他队列，其他连接是不允许建立同名的排他队列的，这个与普通队列不同。
             * 其三，即使该队列是持久化的，一旦连接关闭或者客户端退出，该排他队列都会被自动删除的。
             * 这种队列适用于只限于一个客户端发送读取消息的应用场景。
             *  Auto-delete:自动删除，如果该队列没有任何订阅的消费者的话，该队列会被自动删除。这种队列适用于临时队列。
             *   Durable:持久化.
             */
            //如果该队列已存在，则会返回true；如果不存在，则会返回Error，但是不会创建新的队列。
         QueueDeclareOk ok=   model.QueueDeclare(_queueName, _durable, _exclusive, _autoDelete, _queueArguments);
            
           
            //绑定队列和exchange
            model.QueueBind(_queueName, _exchangeName, _routingKey);
        }

        /// <summary>
        /// 创建连接for RabbitMQ
        /// </summary>
        /// <returns></returns>
        private IConnection CreateConnection()
        {
            return _factory.CreateConnection();
        }

        public override async Task PublishAsync(Type messageType, object message, System.Threading.CancellationToken cancellationToken = default(CancellationToken))
        {
            
            if (message == null)
                return;

            var data = await JsonConvert.SerializeObjectAsync(new MessageBusData
            {
                Type = messageType.AssemblyQualifiedName,
                Data = await JsonConvert.SerializeObjectAsync(message).AnyContext()
            }).AnyContext();

            IBasicProperties basicProperties = _publisherChannel.CreateBasicProperties();
            //basicProperties.Persistent = _persistent;
           // basicProperties.SetPersistent(_persistent);
            byte bt=_persistent == true ? Convert.ToByte(2) : Convert.ToByte(1);
            basicProperties.DeliveryMode = bt;
            
            basicProperties.Expiration = _defaultMessageTimeToLive.Milliseconds.ToString();

           
            // The publication occurs with mandatory=false
            _publisherChannel.BasicPublish(_exchangeName, _routingKey, basicProperties, Encoding.UTF8.GetBytes(data));
        }
        public override void Subscribe<T>(Func<T, System.Threading.CancellationToken, Task> handler, System.Threading.CancellationToken cancellationToken = default(CancellationToken))
        {
            var consumer = new EventingBasicConsumer(_subscriberChannel);
            
            consumer.Received += OnMessageAsync;
            _subscriberChannel.BasicConsume(_queueName, _noAck, consumer);
            base.Subscribe<T>(handler, cancellationToken);//添加到消费者集合中
        }

            private async void OnMessageAsync(object sender, BasicDeliverEventArgs e) 
            {
                var message = await JsonConvert.DeserializeObjectAsync<MessageBusData>(Encoding.UTF8.GetString(e.Body)).AnyContext();
                Type messageType;
                try {
                    messageType = Type.GetType(message.Type);
                } catch (Exception ex) {
                    return;
                }

            object body = await JsonConvert.DeserializeObjectAsync(message.Data, messageType,null).AnyContext();
            await SendMessageToSubscribersAsync(messageType, body).AnyContext();//广播给所有消费者集合中的所有消费者
        }
         public override void Dispose() {
            base.Dispose();
            CloseConnection();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void CloseConnection() {
            if (_subscriberChannel.IsOpen)
                _subscriberChannel.Close();
            _subscriberChannel.Dispose();

            if (_subscriberClient.IsOpen)
                _subscriberClient.Close();
            _subscriberClient.Dispose();

            if (_publisherChannel.IsOpen)
                _publisherChannel.Close();
            _publisherChannel.Dispose();

            if (_publisherClient.IsOpen)
                _publisherClient.Close();
            _publisherClient.Dispose();
        }
    
    }
}
