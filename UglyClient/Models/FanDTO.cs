namespace EnvironmentSimulation.Models
{
    /// <summary>
    /// Data Transfer Object for Fan state information.
    /// </summary>
    public class FanDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier for the fan.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the fan is turned on.
        /// </summary>
        public bool IsOn { get; set; }
    }
}
