using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MQTTnet.Client.Subscribing;
using Xunit;

namespace Mqtt.CustomLibrary.Tests.Unit
{
    public partial class MqttClientTests
    {
        const string topicToSubscribe = "testtopic/#";
        const int qos = 2;
        string[] topicListToSubscribe = new string[] { "topic1/#", "topic2/#" };
        int[] qosList = new int[] { 0, 0 };

        [Fact]
        public async Task ShouldThrowInvalidOperationExceptionWhenNotConnectedWhileSubscribing()
        {
            //given

            //when
            var subscribingResult = _mqttClient.Subscribe(topicToSubscribe, qos);

            //then
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                subscribingResult);
        }

        [Fact]
        public async Task ShouldThrowIArgumentNullExceptionWhenTopicIsNullWhileSubscribing()
        {
            //given
            string nullTopic = null;

            await _mqttClient.Connect();

            //when
            var subscribingResult = _mqttClient.Subscribe(nullTopic, qos);

            //then
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                subscribingResult);
        }

        [Fact]
        public async Task ShouldSubscribeSuccessfully()
        {
            //given
            await _mqttClient.Connect();

            var expectedResultCode = MqttClientSubscribeResultCode.GrantedQoS2;

            //when
            var subscribingResult = await _mqttClient.Subscribe(topicToSubscribe, qos);
            var resultCode = subscribingResult.Items.Select(x => x.ResultCode).FirstOrDefault();

            //then
            resultCode.Should().Be(expectedResultCode);
        }

        [Fact]
        public async Task ShouldSubscribeToMultipleTopicsSuccessfully()
        {
            //given
            await _mqttClient.Connect();
            var expectedResultCode = MqttClientSubscribeResultCode.GrantedQoS2;

            //when
            var multipleSubscriptionResults = await _mqttClient.Subscribe(topicListToSubscribe);

            //then
            multipleSubscriptionResults.ForEach(x => x.Items.Select(x => x.ResultCode).FirstOrDefault().Should().Be(expectedResultCode));
        }

        [Fact]
        public async Task ShouldSubscribeToMultipleTopicsWithQosSuccessfully()
        {
            //given
            await _mqttClient.Connect();
            var expectedResultCode = MqttClientSubscribeResultCode.GrantedQoS0;

            //when
            var multipleSubscriptionResults = await _mqttClient.Subscribe(topicListToSubscribe, qosList);

            //then
            multipleSubscriptionResults.ForEach(x => x.Items.Select(x => x.ResultCode).FirstOrDefault().Should().Be(expectedResultCode));
        }

        [Fact]
        public async Task ShouldSubscribeWithTopicNameOnlySuccessfully()
        {
            //given
            await _mqttClient.Connect();
            var expectedResultCode = MqttClientSubscribeResultCode.GrantedQoS2;

            //when
            var topicSubscriptionResult = await _mqttClient.Subscribe(topicToSubscribe);
            var resultCode = topicSubscriptionResult.Items.Select(x => x.ResultCode).FirstOrDefault();

            //then
            resultCode.Should().Be(expectedResultCode);
        }
    }
}
