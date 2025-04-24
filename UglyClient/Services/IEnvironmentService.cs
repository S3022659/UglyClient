using System.Threading.Tasks;

namespace EnvironmentSimulation.Services
{
    /// <summary>
    /// Interface for environment-related services that interact with sensors, fans, and heaters.
    /// </summary>
    public interface IEnvironmentService
    {
        /// <summary>
        /// Gets the temperature reading from a specified sensor.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor to read.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the temperature in degrees Celsius.</returns>
        Task<double> GetTemperature(int sensorId);

        /// <summary>
        /// Sets the state of a fan (on or off).
        /// </summary>
        /// <param name="fanId">The ID of the fan to control.</param>
        /// <param name="state">The target state of the fan (true for on, false for off).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetFanState(int fanId, bool state);

        /// <summary>
        /// Sets the power level of a heater.
        /// </summary>
        /// <param name="heaterId">The ID of the heater to control.</param>
        /// <param name="level">The target power level (0-5, where 0 is off).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetHeaterLevel(int heaterId, int level);
    }
}