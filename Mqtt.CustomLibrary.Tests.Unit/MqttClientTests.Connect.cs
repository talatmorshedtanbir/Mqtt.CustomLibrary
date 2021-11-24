using System.Threading.Tasks;
using FluentAssertions;
using MQTTnet.Adapter;
using MQTTnet.Client.Connecting;
using Xunit;

namespace Mqtt.CustomLibrary.Tests.Unit
{
    public partial class MqttClientTests
    {
        string mockFalsePassword = "fake12345";
        string mockTruePassword = "12345678";

        [Fact]
        public async Task ShouldThrowMqttConnectingFailedExceptionWhenConnectWithInvalidInfo()
        {
            //given
            var mockClient = new MqttClient(brokerHost, brokerPort, clientId,
                userName, mockFalsePassword,
                cleanSession,
                keepAlivePeriod,
                mqttVersion);

            //when
            var connectResult = mockClient.Connect();

            //then
            await Assert.ThrowsAsync<MqttConnectingFailedException>(() => connectResult);
        }

        [Fact]
        public async Task ShouldConnectSuccessfully()
        {
            //given
            var mockClient = new MqttClient(brokerHost, brokerPort, clientId,
                userName, mockTruePassword,
                cleanSession,
                keepAlivePeriod,
                mqttVersion);
            var expectedConnectionResult = MqttClientConnectResultCode.Success;

            //when
            var connectResult = await mockClient.Connect();

            //then
            connectResult.ResultCode.Should().Be(expectedConnectionResult);
        }
    }
}
