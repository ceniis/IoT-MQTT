using System.Text.Json;
using IoT_MQTT;

namespace MqttConnectionApp
{
    public static class StorageManager
    {
        private static readonly string FilePath = "iot_data.json"; // !add a default path
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };

        public static List<Topic> LoadData()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("Data file is not found. Creating a new initialization state...");
                var defaultList = new List<Topic>
                {
                    new Topic("factory/test_topic")
                    {
                        Devices = new List<Device> { new Device("DEV_001", "TemperatureSensor") }
                    }
                };
                SaveData(defaultList);
                return defaultList;
            }

            try
            {
                string jsonString = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<Topic>>(jsonString, Options) ?? new List<Topic>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON file: {ex.Message}. Starting fresh");
                return new List<Topic>();
            }
        }

        public static void SaveData(List<Topic> topics)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(topics, Options);
                File.WriteAllText(FilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON configuration: {ex.Message}");
            }
        }
    }
}