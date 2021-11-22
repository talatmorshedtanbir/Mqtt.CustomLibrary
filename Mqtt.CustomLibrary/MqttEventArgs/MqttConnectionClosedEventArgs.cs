using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;

namespace Mqtt.CustomLibrary.MqttEventArgs
{
    public class MqttConnectionClosedEventArgs : EventArgs
    {
        public bool ClientWasConnected { get; set; }

        public Exception Exception { get; set; }

        public MqttClientConnectResult ConnectResult { get; set; }

        public MqttClientDisconnectReason Reason { get; set; }

        [Obsolete("Please use 'Reason' instead. This property will be removed in the future!")]
        public MqttClientDisconnectReason ReasonCode { get; set; }
    }
}
