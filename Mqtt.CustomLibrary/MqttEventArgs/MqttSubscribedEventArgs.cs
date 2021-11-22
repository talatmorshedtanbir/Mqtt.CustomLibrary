using MQTTnet.Client.Subscribing;

namespace Mqtt.CustomLibrary.MqttEventArgs
{
    public class MqttSubscribedEventArgs : EventArgs
    {
        public List<MqttClientSubscribeResultItem> Items
        {
            get;
            set;
        } = new List<MqttClientSubscribeResultItem>();
    }
}
