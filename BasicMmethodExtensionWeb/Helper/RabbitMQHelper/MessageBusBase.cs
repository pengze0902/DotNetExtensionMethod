using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BasicMmethodExtensionWeb.Helper.RabbitMQHelper
{
    public abstract class MessageBusBase : MaintenanceBase, IMessageBus
    {
        protected readonly ConcurrentDictionary<string, Subscriber> _subscribers = new ConcurrentDictionary<string, Subscriber>();
        public virtual void Dispose()
        {
            this._subscribers.Clear();
            this.Dispose();
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task PublishAsync(Type messageType, object message, CancellationToken cancellationToken = default(CancellationToken)); //TimeSpan? delay = null,


        /// <summary>
        /// 发送消息给订阅者
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task SendMessageToSubscribersAsync(Type messageType, object message)
        {
            if (message == null)
            {
                return;
            }

            var messageTypeSubscribers = _subscribers.Values.Where(s => s.Type == messageType).ToList();
            foreach (var subscriber in messageTypeSubscribers)
            {
                if (subscriber.CancellationToken.IsCancellationRequested)
                {//如果取消发送则移除订阅者
                    Subscriber sub;
                    continue;
                }
                try
                {
                    await Task.FromResult(subscriber.Action(message, subscriber.CancellationToken));
                }
                catch (Exception ex)
                {
                }
            }

        }


        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="cancellationToken"></param>
        public virtual void Subscribe<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            var subscriber = new Subscriber
            {
                CancellationToken = cancellationToken,
                Type = typeof(T),
                Action = async (message, token) =>
                {
                    if (!(message is T))
                        return;

                    await Task.FromResult(handler((T)message, cancellationToken));
                }
            };
        }
    }

    internal class DelayedMessage
    {
        public DateTime SendTime { get; set; }

        public Type MessageType { get; set; }

        public object Message { get; set; }
    }
    public class Subscriber
    {
        public string Id
        {
            get { return Guid.NewGuid().ToString(); }
            private set { }
        }
        public CancellationToken CancellationToken { get; set; }

        public Type Type { get; set; }

        public Func<object, CancellationToken, Task> Action { get; set; }
    }
}
