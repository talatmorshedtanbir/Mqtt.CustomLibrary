using MQTTnet.Client.Connecting;

namespace Mqtt.CustomLibrary.MqttEventArgs
{
    public class MqttConnectedEventArgs : EventArgs
    {
        public MqttClientConnectResult ConnectResult { get; set; }
    }
}
