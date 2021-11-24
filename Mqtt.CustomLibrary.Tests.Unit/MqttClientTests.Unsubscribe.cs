using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MQTTnet.Client.Unsubscribing;
using Xunit;

namespace Mqtt.CustomLibrary.Tests.Unit
{
    public partial class MqttClientTests
    {
        const string topicToUnsubscribe = "testtopic/#";
        string[] topicListToUnsubscribe = new string[] { "topic1/#", "topic2/#" };

        [Fact]
        public async Task ShouldThrowInvalidOperationExceptionWhenNotConnectedWhileUnsubscribing()
        {
            //given

            //when
            var unsubscribingResult = _mqttClient.Unsubscribe(topicToUnsubscribe);

            //then
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                unsubscribingResult);
        }

        [Fact]
        public async Task ShouldThrowIArgumentNullExceptionWhenTopicIsNullWhileUnsubscribing()
        {
            //given
            string nullTopic = null;
            await _mqttClient.Connect();

            //when
            var unsubscribingResult = _mqttClient.Unsubscribe(nullTopic);

            //then
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                unsubscribingResult);
        }

        [Fact]
        public async Task ShouldUnsubscribeSuccessfully()
        {
            //given
            if (!_mqttClient.IsClientConnected)
            {
                await _mqttClient.Connect();
            }

            var expectedResultCode = MqttClientUnsubscribeResultCode.Success;

            //when
            var unsubscribingResult = await _mqttClient.Unsubscribe(topicToUnsubscribe);
            var resultCode = unsubscribingResult.Items.Select(x => x.ReasonCode).FirstOrDefault();

            //then
            resultCode.Should().Be(expectedResultCode);
        }

        [Fact]
        public async Task ShouldUnsubscribeToTopicListSuccessfully()
        {
            //given
            var expectedResult = MqttClientUnsubscribeResultCode.Success;
            await _mqttClient.Connect();

            //when
            var unsubscriptionResults = await _mqttClient.Unsubscribe(topicListToUnsubscribe);

            //then
            unsubscriptionResults.ForEach(x => x.Items.Select(y => y.ReasonCode).FirstOrDefault().Should().Be(expectedResult));
        }

        [Fact]
        public async Task ShouldReturnNoSubscriptionExistedForTopicsWithNoSubscription()
        {
            //given
            var expectedResult = MqttClientUnsubscribeResultCode.NoSubscriptionExisted;
            var mockTopic = "notSubscribedTopic/#";
            await _mqttClient.Connect();

            //when
            var unsubscriptionResult = await _mqttClient.Unsubscribe(mockTopic);

            //then
            unsubscriptionResult.Items.Select(x => x.ReasonCode.Should().Be(expectedResult));
        }
    }
}
