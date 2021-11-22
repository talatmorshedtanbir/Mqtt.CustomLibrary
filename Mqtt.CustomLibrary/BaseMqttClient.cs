using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mqtt.CustomLibrary.Enums;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;

namespace Mqtt.CustomLibrary
{
    public class BaseMqttClient
    {
        protected MqttClientOptionsBuilder clientOptionsBuilder = new MqttClientOptionsBuilder();

        public BaseMqttClient(string brokerHost,
            int brokerPort,
            string clientId,
            string userName,
            string password,
            bool cleanSession,
            int keepAlivePeriod,
            MqttVersion mqttVersion)
        {
            clientOptionsBuilder.WithTcpServer(brokerHost, brokerPort)
                .WithClientId(clientId)
                .WithCredentials(userName, password)
                .WithCleanSession(cleanSession)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(keepAlivePeriod))
                .WithProtocolVersion((MqttProtocolVersion)mqttVersion);
        }
    }
}
