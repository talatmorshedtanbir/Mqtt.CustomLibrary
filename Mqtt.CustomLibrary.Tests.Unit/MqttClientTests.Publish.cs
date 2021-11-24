using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MQTTnet.Client.Publishing;
using MQTTnet.Exceptions;
using Xunit;

namespace Mqtt.CustomLibrary.Tests.Unit
{
    public partial class MqttClientTests
    {
        const string publishTopic = "testtopic/1";

        [Fact]
        public async Task ShouldPublishSuccessfully()
        {
            // given
            await _mqttClient.Connect();
            var reasonCode = MqttClientPublishReasonCode.Success;
            var randomPayload = GetRandomPayload();

            //when
            var subscribingResult = await _mqttClient.Publish(publishTopic, randomPayload);
            var resultCode = subscribingResult.ReasonCode;

            //then
            resultCode.Should().Be(reasonCode);
        }

        [Fact]
        public async Task ShouldThrowExceptionIfTopicIsInvalid()
        {
            // given
            var topic = "topic/#";
            var randomPayload = GetRandomPayload();
            await _mqttClient.Connect();

            //when
            var subscribingResult = _mqttClient.Publish(topic, randomPayload, true, 2);

            //then
            await Assert.ThrowsAsync<MqttProtocolViolationException>(() =>
                subscribingResult);
        }

        [Fact]
        public async Task ShouldThrowInvalidOperationExceptionIfClientIsNotConnected()
        {
            // given
            var randomPayload = GetRandomPayload();

            //when
            var subscribingResult = _mqttClient.Publish(publishTopic, randomPayload, true, 2);

            //then
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                subscribingResult);
        }

        [Fact]
        public async Task ShouldPublishSuccessfullyWithUserPropertiesKeyValuePair()
        {
            // given
            KeyValuePair<string, string> valuePair = new KeyValuePair<string, string>("Client", "Mock Client");
            var expectedResultCode = MqttClientPublishReasonCode.Success;
            var randomPayload = GetRandomPayload();
            await _mqttClient.Connect();

            //when
            var publishResult = await _mqttClient.Publish(publishTopic, randomPayload, valuePair);
            var resultCode = publishResult.ReasonCode;

            //then
            resultCode.Should().Be(expectedResultCode);
        }

        [Fact]
        public async Task ShouldPublishSuccessfullyWithRetainFlag()
        {
            // given
            var retainFlag = false;
            var qosLevel = 1;
            var randomPayload = GetRandomPayload();
            var expectedResultCode = MqttClientPublishReasonCode.Success;
            await _mqttClient.Connect();

            //when
            var publishResult = await _mqttClient.Publish(publishTopic, randomPayload, retainFlag, qosLevel);
            var resultCode = publishResult.ReasonCode;

            //then
            resultCode.Should().Be(expectedResultCode);
        }

        [Fact]
        public async Task ShouldPublishSuccessfullyWithQos()
        {
            // given
            var qosLevel = 1;
            var randomPayload = GetRandomPayload();
            var expectedResultCode = MqttClientPublishReasonCode.Success;
            await _mqttClient.Connect();

            //when
            var publishResult = await _mqttClient.Publish(publishTopic, randomPayload, qosLevel);
            var resultCode = publishResult.ReasonCode;

            //then
            resultCode.Should().Be(expectedResultCode);
        }
    }
}
