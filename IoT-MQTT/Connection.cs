using MQTTnet;
using System.Buffers;
using System.Text;

namespace MqttConnectionApp
{
    public class MqttService
    {
        private IMqttClient _mqttClient;
        private MqttClientOptions _mqttOptions;

        public bool IsConnected => _mqttClient?.IsConnected ?? false;

        public async Task ConnectAsync()
        {
            var clientFactory = new MqttClientFactory();
            _mqttClient = clientFactory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com", 1883)
                .WithClientId($"CSharp_Client_{Guid.NewGuid()}")
                .WithCleanSession()
                .Build();
           
            _mqttClient.ConnectedAsync += OnConnectedAsync;
            _mqttClient.DisconnectedAsync += OnDisconnectedAsync;
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;

            try
            {
                Console.WriteLine("Connecting to MQTT broker...");
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        public async Task DisconnectAsync()
        {
            if (_mqttClient != null)
            {
                await _mqttClient.DisconnectAsync();
            }
        }

        public async Task SubscribeAsync(string topic)
        {
            if (!IsConnected)
            {
                Console.WriteLine("Cannot subscribe. Not connected to broker");
                return;
            }

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(topic))
                .Build();

            await _mqttClient.SubscribeAsync(subscribeOptions, CancellationToken.None);
            Console.WriteLine($"Successfully subscribed to topic: {topic}");
        }

        public async Task UnsubscribeAsync(string topic)
        {
            if (!IsConnected)
            {
                Console.WriteLine("Cannot unsubscribe. Not connected to broker");
                return;
            }

            var unsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            await _mqttClient.UnsubscribeAsync(unsubscribeOptions, CancellationToken.None);
            Console.WriteLine($"Successfully unsubscribed from topic: {topic}");
        }

        public async Task PublishAsync(string topic, string payload)
        {
            if (!IsConnected)
            {
                Console.WriteLine("Cannot publish. Not connected to broker");
                return;
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await _mqttClient.PublishAsync(message, CancellationToken.None);
            Console.WriteLine($"Published message to [{topic}]");
        }

        private Task OnConnectedAsync(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("[Event] Connection established with broker");
            return Task.CompletedTask;
        }

        private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine("[Event] Disconnected from broker");
            return Task.CompletedTask;
        }

        private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload.ToArray());
            Console.WriteLine($"\n[Incoming Message] Topic: {e.ApplicationMessage.Topic} | Payload: {payload}\n");
            return Task.CompletedTask;
        }
    }
}