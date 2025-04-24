using System.Net.Http;
using System.Threading.Tasks;
using EnvironmentSimulation.Models;

namespace EnvironmentSimulation.Services
{
    /// <summary>
    /// Implementation of the environment service that communicates with the environment simulation API.
    /// </summary>
    public class EnvironmentService : IEnvironmentService
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentService"/> class.
        /// </summary>
        /// <param name="client">The HTTP client used to communicate with the API.</param>
        public EnvironmentService(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets the temperature reading from a specified sensor.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor to read.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the temperature in degrees Celsius.</returns>
        public async Task<double> GetTemperature(int sensorId)
        {
            var response = await _client.GetAsync($"api/sensor/{sensorId}");
            response.EnsureSuccessStatusCode();
            var tempString = await response.Content.ReadAsStringAsync();
            return double.Parse(tempString);
        }

        /// <summary>
        /// Sets the state of a fan (on or off).
        /// </summary>
        /// <param name="fanId">The ID of the fan to control.</param>
        /// <param name="state">The target state of the fan (true for on, false for off).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetFanState(int fanId, bool state)
        {
            var response = await _client.PostAsync($"api/fans/{fanId}",
                new StringContent(state.ToString().ToLower(), System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Sets the power level of a heater.
        /// </summary>
        /// <param name="heaterId">The ID of the heater to control.</param>
        /// <param name="level">The target power level (0-5, where 0 is off).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetHeaterLevel(int heaterId, int level)
        {
            var response = await _client.PostAsync($"api/heat/{heaterId}",
                new StringContent(level.ToString(), System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }
    }
}
