using IoT_MQTT;

namespace MqttConnectionApp
{
    class Program
    {

        public static void AddDevice(Topic topic)
        {
            Console.WriteLine("Enter Device ID:");
            string devId = Console.ReadLine();
            Console.WriteLine("Enter Device Type:");
            string devType = Console.ReadLine();
            topic.Devices.Add(new Device(devId, devType));
            Console.WriteLine($"Device '{devId}' saved successfully");
        }

        static async Task Main(string[] args)
        {
            List<Topic> managedTopics = StorageManager.LoadData();

            var mqttService = new MqttService();
            await mqttService.ConnectAsync();

            foreach (var topic in managedTopics)
            {
                if (topic.IsSubscribed)
                {
                    await mqttService.SubscribeAsync(topic.Name);
                }
            }

            while (true)
            {
                Console.WriteLine("\n0 - Exit\t 1 - Create new Topic\t 2 - Subscribe\t 3 - Publish\t 4 - Unsubscribe\t 5 - List of Topics\n");
                Console.WriteLine("6 - Add Device to Topic\t 7 - View logs\t");
                Console.Write("Enter choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid entry");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter the topic name:");
                        var topicName = Console.ReadLine();

                        if (managedTopics.Any(t => t.Name.Equals(topicName, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Topic already exists in records");
                            break;
                        }

                        Topic newTopic = new Topic(topicName);

                        Console.WriteLine("Add a device to this topic? (y/n)");
                        if (Console.ReadLine()?.ToLower() == "y")
                        {
                            AddDevice(newTopic);
                        }

                        managedTopics.Add(newTopic);
                        StorageManager.SaveData(managedTopics);
                        Console.WriteLine($"Topic '{topicName}' is created successfully");
                        break;

                    case 2:
                        Console.WriteLine("Enter the topic name to subscribe:");
                        topicName = Console.ReadLine();
                        var targetSubTopic = managedTopics.FirstOrDefault(t => t.Name == topicName);
                        
                        if (targetSubTopic != null)
                        {
                            await targetSubTopic.SubscribeAsync(mqttService);
                            StorageManager.SaveData(managedTopics);
                        }
                        else Console.WriteLine($"There is no existing topic'{topicName}'. Create it first");
                       
                        break;

                    case 3:
                        Console.WriteLine("Enter the topic name:");
                        topicName = Console.ReadLine();
                        Console.WriteLine("Enter the message:");
                        var message = Console.ReadLine();

                        var targetPubTopic = managedTopics.FirstOrDefault(t => t.Name == topicName) ?? new Topic(topicName);
                        await targetPubTopic.PublishAsync(mqttService, message);
                        break;

                    case 4:
                        Console.WriteLine("Enter the topic name to unsubscribe:");
                        topicName = Console.ReadLine();
                        var targetUnsubTopic = managedTopics.FirstOrDefault(t => t.Name == topicName);
                       
                        if (targetUnsubTopic != null)
                        {
                            await targetUnsubTopic.UnsubscribeAsync(mqttService);
                            StorageManager.SaveData(managedTopics);
                        }

                        break;

                    case 5:
                        if (managedTopics.Count == 0)
                        {
                            Console.WriteLine("No tracked topics currently exist");
                        }
                        foreach (var t in managedTopics)
                        {
                            Console.WriteLine($"Topic: {t.Name}");
                            foreach (var d in t.Devices)
                            {
                                Console.WriteLine($"  └── Device: ID={d.DeviceId} | Type={d.DeviceType} | Status={d.Status}");
                            }
                        }
                        break;

                    case 0:
                        Console.WriteLine("Disconnecting...");
                        await mqttService.DisconnectAsync();
                        Console.WriteLine("Exit.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}