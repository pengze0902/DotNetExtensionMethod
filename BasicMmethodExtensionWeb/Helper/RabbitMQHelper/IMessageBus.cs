using System;

namespace BasicMmethodExtensionWeb.Helper.RabbitMQHelper
{
   public interface IMessageBus: IMessagePublisher, IMessageSubscriber, IDisposable 
    {
    }
}
