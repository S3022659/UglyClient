namespace EnvironmentSimulation.Models
{
    /// <summary>
    /// Represents the type of device in the environment simulation.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// A temperature sensor device.
        /// </summary>
        Sensor,
        
        /// <summary>
        /// A cooling fan device.
        /// </summary>
        Fan,
        
        /// <summary>
        /// A heating device.
        /// </summary>
        Heater
    }

    /// <summary>
    /// Represents a device in the environment simulation system.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Gets or sets the unique identifier for the device.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public required string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        public DeviceType Type { get; set; }
    }
}
