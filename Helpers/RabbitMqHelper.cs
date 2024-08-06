#nullable enable
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Coaction.KickAssCardBot.Helpers
{
    public static class RabbitMqHelper
    {
        //private static readonly IModel ChannelForEventing = null!;

        //public static void RecieveMoversAndShakersCardInfo(object? sender, BasicDeliverEventArgs e)
        //{
        //    IBasicProperties basicProperties = e.BasicProperties;
        //    Logger.Log.Info("Message received by the event based consumer. Check the debug window for details.");
        //    Logger.Log.Info(string.Concat("Message received from the exchange ", e.Exchange));
        //    Logger.Log.Info(string.Concat("Content type: ", basicProperties.ContentType));
        //    Logger.Log.Info(string.Concat("Consumer tag: ", e.ConsumerTag));
        //    Logger.Log.Info(string.Concat("Delivery tag: ", e.DeliveryTag));
        //    var body = e.Body.ToArray();
        //    var message = Encoding.UTF8.GetString(body);
        //    ChannelForEventing.BasicAck(e.DeliveryTag, false);
        //}
    }
}
