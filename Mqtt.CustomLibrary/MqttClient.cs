using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mqtt.CustomLibrary.Enums;
using Mqtt.CustomLibrary.MqttEventArgs;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Protocol;

namespace Mqtt.CustomLibrary
{
    public sealed class MqttClient : BaseMqttClient
    {
        #region Private variables
        private IMqttClient _mqttClient;
        private IMqttClientOptions mqttClientOptions;

        private readonly MqttQualityOfServiceLevel defaultQosLevel = MqttQualityOfServiceLevel.ExactlyOnce;
        private readonly MqttQualityOfServiceLevel subscribeQoSLevel = MqttQualityOfServiceLevel.ExactlyOnce;
        private readonly MqttQualityOfServiceLevel publishQoSLevel = MqttQualityOfServiceLevel.ExactlyOnce;

        private bool _disposed = false;
        private bool _isClientDisconnected = false;
        #endregion

        #region Constructors
        public MqttClient(string brokerHost,
            int brokerPort,
            string clientId,
            string userName,
            string password,
            bool cleanSession,
            int keepAlivePeriod,
            MqttVersion mqttVersion) : base(brokerHost,
                brokerPort,
                clientId,
                userName,
                password,
                cleanSession,
                keepAlivePeriod,
                mqttVersion)
        {
            mqttClientOptions = clientOptionsBuilder
                .WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true,
                    AllowUntrustedCertificates = true
                })
                .Build();
        }

        public MqttClient(string brokerHost,
            int brokerPort,
            string clientId,
            string userName,
            string password,
            string caCertificate,
            bool cleanSession,
            int keepAlivePeriod,
            MqttVersion mqttVersion,
            SslProtocols tlsVersion) : base(brokerHost,
                brokerPort,
                clientId,
                userName,
                password,
                cleanSession,
                keepAlivePeriod,
                mqttVersion)
        {
            var caCertificateFile = new X509Certificate2(caCertificate);

            mqttClientOptions = clientOptionsBuilder
                .WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true,
                    SslProtocol = tlsVersion,
                    AllowUntrustedCertificates = true,
                    Certificates = new List<X509Certificate>() { caCertificateFile }
                })
                .Build();
        }

        public MqttClient(string brokerHost,
            int brokerPort,
            string clientId,
            string userName,
            string password,
            string clientCertificate,
            string clientkey,
            bool cleanSession,
            int keepAlivePeriod,
            MqttVersion mqttVersion,
            SslProtocols tlsVersion) : base(brokerHost,
            brokerPort,
            clientId,
            userName,
            password,
            cleanSession,
            keepAlivePeriod,
            mqttVersion)
        {
            var clientCertificateFile = new X509Certificate2(clientCertificate, clientkey);

            mqttClientOptions = clientOptionsBuilder
                .WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true,
                    SslProtocol = tlsVersion,
                    Certificates = new List<X509Certificate>() { clientCertificateFile }
                })
                .Build();
        }

        public MqttClient(string brokerHost,
            int brokerPort,
            string clientId,
            string userName,
            string password,
            string caCertificate,
            string clientCertificate,
            string clientkey,
            bool cleanSession,
            int keepAlivePeriod,
            MqttVersion mqttVersion,
            SslProtocols tlsVersion) : base(brokerHost,
            brokerPort,
            clientId,
            userName,
            password,
            cleanSession,
            keepAlivePeriod,
            mqttVersion)
        {
            var caCertificateFile = X509Certificate.CreateFromCertFile(caCertificate);
            var clientCertificateFile = new X509Certificate2(clientCertificate, clientkey);

            mqttClientOptions = clientOptionsBuilder
                .WithTls(new MqttClientOptionsBuilderTlsParameters()
                {
                    UseTls = true,
                    SslProtocol = tlsVersion,
                    Certificates = new List<X509Certificate>()
                    {
                        caCertificateFile,
                        clientCertificateFile
                    }
                })
                .Build();
        }
        #endregion

        #region Connection Region
        public async Task<MqttClientConnectResult> Connect()
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
                _mqttClient.Dispose();
            }

            _mqttClient = new MqttFactory().CreateMqttClient();

            InitEventHandlers();

            MqttClientConnectResult result =
                await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            ThrowIfNotConnected();

            _isClientDisconnected = false;

            return result;
        }

        public async Task Disconnect()
        {
            ThrowIfNotConnected();
            MqttDisconnectedEventHandler();

            _isClientDisconnected = true;

            await _mqttClient.DisconnectAsync();
        }

        #endregion

        #region Subscription Region

        /// </summary>
        /// <param name="topic, qos">Topic to subscribe</param>
        /// <param name="qos">QoS level of the subcribed topic</param>
        public async Task<MqttClientSubscribeResult> Subscribe(string topic, int qos)
        {
            ValidateParameters(new List<object>() { topic });

            ThrowIfNotConnected();

            var qosLevel = GetQosLevel(qos);

            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel(qosLevel)
                .Build();

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topicFilter)
                .Build();

            var subscribeResult = await _mqttClient.SubscribeAsync(subscribeOptions).ConfigureAwait(false);

            MqttSubscribedEventHandler(subscribeResult);

            return subscribeResult;
        }

        /// </summary>
        /// <param name="topics">Collection of topics to subscribe</param>
        /// <param name="qosList">QoS level of the subcribed topics collection</param>
        public async Task<List<MqttClientSubscribeResult>> Subscribe(string[] topics, int[] qosList)
        {
            var subscribeResults = new List<MqttClientSubscribeResult>();

            ValidateParameters(new List<object>() { topics });

            for (int i = 0; i < topics.Length; i++)
            {
                var subscribeResult = await Subscribe(topics[i], qosList[i]);
                subscribeResults.Add(subscribeResult);
            }

            return subscribeResults;
        }

        /// </summary>
        /// <param name="topics">Collection of topics to subscribe</param>
        public async Task<List<MqttClientSubscribeResult>> Subscribe(string[] topics)
        {
            ValidateParameters(new List<object>() { topics });

            int[] qosLevels = new int[topics.Length];

            for (int i = 0; i < topics.Length; i++)
            {
                qosLevels[i] = (int)subscribeQoSLevel;
            }

            var subscribeResults = await Subscribe(topics, qosLevels);

            return subscribeResults;
        }

        /// </summary>
        /// <param name="topic">Topic to subscribe</param>
        public async Task<MqttClientSubscribeResult> Subscribe(string topic)
        {
            ValidateParameters(new List<object>() { topic });

            var subscribeResult = await Subscribe(topic, (int)subscribeQoSLevel);

            return subscribeResult;
        }

        public async Task<MqttClientUnsubscribeResult> Unsubscribe(string topic)
        {
            ValidateParameters(new List<object>() { topic });

            ThrowIfNotConnected();

            var unsubscribeResult = await _mqttClient.UnsubscribeAsync(topic);

            MqttUnsubscribedEventHandler(unsubscribeResult);

            return unsubscribeResult;
        }

        public async Task<List<MqttClientUnsubscribeResult>> Unsubscribe(string[] topics)
        {
            var unsubscriptionResults = new List<MqttClientUnsubscribeResult>();

            ValidateParameters(new List<object>() { topics });

            ThrowIfNotConnected();

            foreach (var topic in topics)
            {
                var unsubscriptionResult = await Unsubscribe(topic);
                unsubscriptionResults.Add(unsubscriptionResult);
            }

            return unsubscriptionResults;
        }

        #endregion

        #region Publish

        public async Task<MqttClientPublishResult> Publish(string topic, string payload)
        {
            var result = await Publish(topic, payload, retainFlag: true, qos: 2);

            return result;
        }

        public async Task<MqttClientPublishResult> Publish(string topic, string payload, int qos = 2)
        {
            var result = await Publish(topic, payload, retainFlag: true, qos);

            return result;
        }

        public async Task<MqttClientPublishResult> Publish(string topic, string payload, bool retainFlag, int qos = 2)
        {
            ValidateParameters(new List<object>() { topic });

            ThrowIfNotConnected();

            var qosLevel = GetQosLevel(qos);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel(qosLevel)
                .WithRetainFlag(retainFlag)
                .WithPayload(payload)
                .Build();

            var result = await _mqttClient.PublishAsync(applicationMessage);

            MqttMessagePublishedEventHandler(result);

            return result;
        }

        public async Task<MqttClientPublishResult> Publish(string topic, string payload, KeyValuePair<string, string> valuePair)
        {
            ValidateParameters(new List<object>() { topic });

            ThrowIfNotConnected();

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel(publishQoSLevel)
                .WithPayload(payload)
                .WithUserProperty(valuePair.Key, valuePair.Value)
                .Build();

            var result = await _mqttClient.PublishAsync(applicationMessage);

            MqttMessagePublishedEventHandler(result);

            return result;
        }

        #endregion

        #region Events

        public event EventHandler<MqttConnectedEventArgs> ClientConnected;
        public event EventHandler<MqttDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<MsgReceivedEventArgs> MessageReceived;
        public event EventHandler<MsgPublishedEventArgs> MessagePublished;
        public event EventHandler<MqttSubscribedEventArgs> ClientSubscribed;
        public event EventHandler<MqttUnsubscribedEventArgs> ClientUnsubscribed;
        public event EventHandler<MqttConnectionClosedEventArgs> ConnectionClosed;

        #endregion

        #region Event Handlers

        private void InitEventHandlers()
        {
            MqttConnectedEventHandler();
            MqttConnectionClosedEventHandler();
            MqttMessageReceivedEventHandler();
        }

        private void MqttConnectedEventHandler()
        {
            _mqttClient.UseConnectedHandler(e =>
            {
                var connectedEventResult = new MqttConnectedEventArgs()
                {
                    ConnectResult = e.ConnectResult
                };

                ClientConnected?.Invoke(this, connectedEventResult);
            });
        }

        private void MqttDisconnectedEventHandler()
        {
            _mqttClient.UseDisconnectedHandler(e =>
            {
                var disconnectedEventResult = new MqttDisconnectedEventArgs()
                {
                    ClientWasConnected = e.ClientWasConnected,
                    ConnectResult = e.ConnectResult,
                    Reason = e.Reason,
                    Exception = e.Exception
                };

                ClientDisconnected?.Invoke(this, disconnectedEventResult);
            });
        }

        private void MqttMessageReceivedEventHandler()
        {
            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                var receivedEventMessage = new MsgReceivedEventArgs()
                {
                    AutoAcknowledge = e.AutoAcknowledge,
                    ReasonCode = e.ReasonCode,
                    ApplicationMessage = e.ApplicationMessage,
                    IsHandled = e.IsHandled,
                    ProcessingFailed = e.ProcessingFailed,
                    Tag = e.Tag,
                    ClientId = e.ClientId
                };

                // Raise message received event handler
                MessageReceived?.Invoke(this, receivedEventMessage);
            });
        }

        private void MqttMessagePublishedEventHandler(MqttClientPublishResult publishResult)
        {
            var publishedEventMessage = new MsgPublishedEventArgs()
            {
                UserProperties = publishResult.UserProperties,
                ReasonCode = publishResult.ReasonCode,
                PacketIdentifier = publishResult.PacketIdentifier,
                ReasonString = publishResult.ReasonString
            };

            MessagePublished?.Invoke(this, publishedEventMessage);
        }

        private void MqttSubscribedEventHandler(MqttClientSubscribeResult subscribedResult)
        {
            var subscribedEventMessage = new MqttSubscribedEventArgs()
            {
                Items = subscribedResult.Items
            };

            ClientSubscribed?.Invoke(this, subscribedEventMessage);
        }

        private void MqttUnsubscribedEventHandler(MqttClientUnsubscribeResult unsubscribeResult)
        {
            var unsubscribedEventMessage = new MqttUnsubscribedEventArgs()
            {
                Items = unsubscribeResult.Items
            };

            ClientUnsubscribed?.Invoke(this, unsubscribedEventMessage);
        }

        private void MqttConnectionClosedEventHandler()
        {
            _mqttClient.UseDisconnectedHandler(e =>
            {
                var connectionClosedEventResult = new MqttConnectionClosedEventArgs()
                {
                    ClientWasConnected = e.ClientWasConnected,
                    ConnectResult = e.ConnectResult,
                    Reason = e.Reason,
                    Exception = e.Exception
                };

                ConnectionClosed?.Invoke(this, connectionClosedEventResult);

                if (!_isClientDisconnected && !_mqttClient.IsConnected)
                {
                    MqttConnectionReconnectOnClose();
                }
            });
        }

        private async void MqttConnectionReconnectOnClose()
        {
            Thread.Sleep(2000);

            await this.Connect();
        }

        #endregion

        #region Helper Methods

        public bool IsClientConnected => _mqttClient?.IsConnected is true;

        void ThrowIfNotConnected()
        {
            if (_mqttClient is null || !_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("The MQTT client is not connected.");
            }
        }

        private MqttQualityOfServiceLevel GetQosLevel(int qos)
        {
            switch (qos)
            {
                case 0:
                    return MqttQualityOfServiceLevel.AtMostOnce;
                case 1:
                    return MqttQualityOfServiceLevel.AtLeastOnce;
                case 2:
                    return MqttQualityOfServiceLevel.ExactlyOnce;
            }

            return defaultQosLevel;
        }

        void ValidateParameters(List<object> parameters)
        {
            parameters.ForEach(
                parameter =>
                {
                    if (parameter is null)
                    {
                        throw new ArgumentNullException($"Value is null for parameter {nameof(parameters)}");
                    }
                });
        }

        #endregion

        #region Object dispose
        // Public implementation of Dispose pattern callable by consumers.
        public async Task Dispose()
        {
            await Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        public async Task Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _mqttClient?.Dispose();
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
                _mqttClient = null;
            }
            _disposed = true;
        }

        // Finalizer by destructor
        ~MqttClient()
        {
            Dispose(true);
        }
        #endregion

    }
}
