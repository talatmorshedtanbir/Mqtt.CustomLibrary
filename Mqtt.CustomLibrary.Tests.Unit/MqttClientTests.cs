using System;
using Mqtt.CustomLibrary.Enums;

namespace Mqtt.CustomLibrary.Tests.Unit
{
    public partial class MqttClientTests
    {
        private MqttClient _mqttClient;

        string brokerHost = "23.97.61.172";
        int brokerPort = 8883;
        string clientId = "DataProcessor_DEBUG";
        string userName = "DataProcessor_DEBUG";
        string password = "12345678";
        bool cleanSession = true;
        int keepAlivePeriod = 50;
        MqttVersion mqttVersion = MqttVersion.V311;

        public MqttClientTests()
        {
            _mqttClient = new MqttClient(brokerHost, brokerPort, clientId,
                    userName, password,
                    cleanSession,
                    keepAlivePeriod,
                    mqttVersion);
        }

        public string GetRandomPayload()
        {
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
            char[] consonants = { 'w', 'r', 't', 'z', 'p', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'c', 'v', 'b', 'n', 'm' };

            var random = new Random();
            int wordLength = random.Next(3, 15);

            string result = string.Empty;


            string currentWord = null;

            bool nextIsVowel = random.Next(0, 1) == 1;

            bool upperLetter = true;

            for (int i = 0; i < wordLength; i++)
            {
                char currentChar;
                if (nextIsVowel)
                {
                    currentChar = vowels[random.Next(0, vowels.Length)];
                }
                else
                {
                    currentChar = consonants[random.Next(0, consonants.Length)];
                }

                currentWord += upperLetter ?
                    char.ToUpper(currentChar) :
                    currentChar;

                upperLetter = false;
                nextIsVowel = !nextIsVowel;
            }

            result += currentWord;

            return result.Trim();
        }

    }
}
