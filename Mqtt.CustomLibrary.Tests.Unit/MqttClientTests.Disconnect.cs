using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Mqtt.CustomLibrary.Tests.Unit
{
    public partial class MqttClientTests
    {
        [Fact]
        public async Task ShouldThrowInvalidOperationExceptionWhenNotConnectBeforeDisconnecting()
        {
            //given
            var mockClientToDisconnect = new MqttClient(brokerHost, brokerPort, clientId,
                    userName, mockFalsePassword,
                    cleanSession,
                    keepAlivePeriod,
                    mqttVersion);

            //when
            var disconnectResult = mockClientToDisconnect.Disconnect();

            //then
            await Assert.ThrowsAsync<InvalidOperationException>(() => disconnectResult);
        }

        [Fact]
        public async Task ShouldDisconnectSuccessfully()
        {
            //given
            var mockClientToDisconnect = new MqttClient(brokerHost, brokerPort, clientId,
                    userName, mockTruePassword,
                    cleanSession,
                    keepAlivePeriod,
                    mqttVersion);
            await mockClientToDisconnect.Connect();
            var expectedConnectionResult = false;

            //when
            await mockClientToDisconnect.Disconnect();

            //then
            mockClientToDisconnect.IsClientConnected.Should().Be(expectedConnectionResult);
        }
    }
}
