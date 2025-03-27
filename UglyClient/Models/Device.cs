namespace EnvironmentSimulation.Models
{
    public enum DeviceType
    {
        Sensor,
        Fan,
        Heater
    }

    public class Device
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DeviceType Type { get; set; }
    }
}
