using MQTTnet.Client.Publishing;
using MQTTnet.Packets;

namespace Mqtt.CustomLibrary.MqttEventArgs
{
    public class MsgPublishedEventArgs : EventArgs
    {
        public ushort? PacketIdentifier { get; set; }

        public MqttClientPublishReasonCode ReasonCode { get; set; }

        public string ReasonString { get; set; }

        public List<MqttUserProperty> UserProperties { get; set; }
    }
}
