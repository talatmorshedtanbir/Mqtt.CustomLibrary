using MQTTnet;
using MQTTnet.Packets;

namespace Mqtt.CustomLibrary.MqttEventArgs
{
    public class MsgReceivedEventArgs : EventArgs
    {
        private readonly Func<MsgReceivedEventArgs, CancellationToken, Task> _acknowledgeHandler;

        private int _isAcknowledged;

        internal MqttPublishPacket PublishPacket { get; set; }

        public string ClientId { get; set; }

        public MqttApplicationMessage ApplicationMessage { get; set; }

        public bool ProcessingFailed { get; set; }

        public MqttApplicationMessageReceivedReasonCode ReasonCode { get; set; }

        public bool IsHandled { get; set; }

        public bool AutoAcknowledge { get; set; } = true;

        public object Tag { get; set; }

        public Task AcknowledgeAsync(CancellationToken cancellationToken)
        {
            if (_acknowledgeHandler == null)
            {
                throw new NotSupportedException("Deferred acknowledgement of application message is not yet supported in MQTTnet server.");
            }

            if (Interlocked.CompareExchange(ref _isAcknowledged, 1, 0) == 0)
            {
                return _acknowledgeHandler(this, cancellationToken);
            }

            throw new InvalidOperationException("The application message is already acknowledged.");
        }
    }
}
