using MQTTnet;
using System.Buffers;
using System.Text;


namespace MqttConnectionApp
{
    class Program
    {
        private static IMqttClient _mqttClient;
        private static MqttClientOptions _mqttOptions;

        static async Task Main(string[] args)
        {
            var clientFactory = new MqttClientFactory();
            _mqttClient = clientFactory.CreateMqttClient();

            // Configure connection parameters
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

                Console.WriteLine("Connected! Press Enter to exit");
                Console.ReadLine();

                await _mqttClient.DisconnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }

        private static async Task OnConnectedAsync(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("Successfully connected to the broker!");
            //
            var topic = "test/topic/csharp";

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(topic))
                .Build();

            await _mqttClient.SubscribeAsync(subscribeOptions, CancellationToken.None);
            Console.WriteLine($"Subscribed to topic: {topic}");
        }

        private static Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Disconnected from the broker.");
            return Task.CompletedTask;
        }

        private static Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload.ToArray());

            Console.WriteLine($"\n[Received] Topic: {e.ApplicationMessage.Topic} | Payload: {payload}");
            return Task.CompletedTask;
        }
    }
}