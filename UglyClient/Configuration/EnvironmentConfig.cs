namespace EnvironmentSimulation.Configuration
{
    /// <summary>
    /// Configuration settings for the environment simulation application.
    /// </summary>
    public class EnvironmentConfig
    {
        /// <summary>
        /// Gets or sets the base URL for the API.
        /// </summary>
        public required string ApiBaseUrl { get; set; }
        
        /// <summary>
        /// Gets or sets the API key used for authentication.
        /// </summary>
        public required string ApiKey { get; set; }
        
        /// <summary>
        /// Gets or sets the number of temperature sensors in the system.
        /// </summary>
        public int NumSensors { get; set; }
        
        /// <summary>
        /// Gets or sets the number of heaters in the system.
        /// </summary>
        public int NumHeaters { get; set; }
        
        /// <summary>
        /// Gets or sets the number of fans in the system.
        /// </summary>
        public int NumFans { get; set; }
    }
}
