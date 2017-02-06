using System;
using System.Threading;
using System.Threading.Tasks;

namespace BasicMmethodExtensionWeb.Helper.RabbitMQHelper
{
   public  interface IMessagePublisher
    {
        Task PublishAsync(Type messageType, object message, CancellationToken cancellationToken = default(CancellationToken));//TimeSpan? delay = null, 
    }
   public static class MessagePublisherExtensions
   {
       public static Task PublishAsync<T>(this IMessagePublisher publisher, T message) where T : class//, TimeSpan? delay = null
       {
           return publisher.PublishAsync(typeof(T), message);//delay
       }
   }
}
