namespace IoT_MQTT
{
    public class Device
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Status { get; set; } = "Offline";

        public Device() { }

        public Device(string deviceId, string deviceType)
        {
            DeviceId = deviceId;
            DeviceType = deviceType;
        }
        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }  
    }

    public class Topic
    {
        public string Name { get; set; }
        public bool IsSubscribed { get; set; } = false;

        public List<Device> Devices { get; set; } = new List<Device>();

        public Topic() { }

        public Topic(string name)
        {
            Name = name;
        }

        public async Task SubscribeAsync(MqttConnectionApp.MqttService mqttService)
        {
            await mqttService.SubscribeAsync(Name);
        }

        public async Task UnsubscribeAsync(MqttConnectionApp.MqttService mqttService)
        {
            await mqttService.UnsubscribeAsync(Name);
        }

        public async Task PublishAsync(MqttConnectionApp.MqttService mqttService, string message)
        {
            await mqttService.PublishAsync(Name, message);
        }
    }
}