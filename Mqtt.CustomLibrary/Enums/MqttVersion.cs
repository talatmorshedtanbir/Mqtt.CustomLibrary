using MQTTnet.Formatter;

namespace Mqtt.CustomLibrary.Enums
{
    public enum MqttVersion
    {
        Unknown = MqttProtocolVersion.Unknown,
        V310 = MqttProtocolVersion.V310,
        V311 = MqttProtocolVersion.V311,
        V500 = MqttProtocolVersion.V500
    }
}
