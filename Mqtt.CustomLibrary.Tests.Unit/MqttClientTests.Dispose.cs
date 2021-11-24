using System;
using System.Threading.Tasks;
using Xunit;

namespace Mqtt.CustomLibrary.Tests.Unit
{
    public partial class MqttClientTests
    {
        [Fact]
        public async Task ShouldDisposeClientSuccessfully()
        {
            //given
            var mockClient = new MqttClient(brokerHost, brokerPort, clientId,
                userName, mockTruePassword,
                cleanSession,
                keepAlivePeriod,
                mqttVersion);

            await mockClient.Connect();

            //when
            var disposingClient = mockClient.Dispose();

            //then
            await Assert.ThrowsAsync<ObjectDisposedException>(() => disposingClient);
        }
    }
}
