using MQTTnet;


namespace MqttConnectionApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mqttService = new MqttService();
            await mqttService.ConnectAsync();

            while (true)
            {
                Console.WriteLine("\n1 - Create new Topic\t 2 - Subscribe\t 3 - Publish\t 4 - Unsubscribe\t 0 - Exit");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter the topic name:");
                        var topicName = Console.ReadLine();
                        // func call
                        Console.WriteLine($"Topic '{topicName}' is created");
                        break;
                    case 2:
                        Console.WriteLine("Enter the topic name:");
                        topicName = Console.ReadLine();
                        // func call
                        Console.WriteLine($"Subscribed to '{topicName}'");
                        break;
                    case 3:
                        Console.WriteLine("Enter the topic name:");
                        topicName = Console.ReadLine();
                        Console.WriteLine("Enter the message:");
                        var message = Console.ReadLine();
                        // func call
                        Console.WriteLine($"Message '{message}'is published to '{topicName}'");
                        break;
                    case 4: 
                        Console.WriteLine("Enter the topic name:");
                        topicName = Console.ReadLine();
                        // func call
                        Console.WriteLine($"Unsubscribed from '{topicName}'");
                        break;
                    case 0:
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }   
        }
    }
}
