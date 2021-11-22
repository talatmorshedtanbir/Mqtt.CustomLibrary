using MQTTnet.Client.Unsubscribing;

namespace Mqtt.CustomLibrary.MqttEventArgs
{
    public class MqttUnsubscribedEventArgs : EventArgs
    {
        public List<MqttClientUnsubscribeResultItem> Items
        {
            get;
            set;
        } = new List<MqttClientUnsubscribeResultItem>();

    }
}
