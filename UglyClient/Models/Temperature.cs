namespace EnvironmentSimulation.Models
{
    /// <summary>
    /// Represents a temperature reading with a value and unit of measurement.
    /// </summary>
    public class Temperature
    {
        /// <summary>
        /// Gets or sets the numeric value of the temperature reading.
        /// </summary>
        public double Value { get; set; }
        
        /// <summary>
        /// Gets or sets the unit of measurement for the temperature (e.g., "C" for Celsius, "F" for Fahrenheit).
        /// </summary>
        public required string Unit { get; set; }
    }
}
