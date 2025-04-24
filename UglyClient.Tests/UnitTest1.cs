using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EnvironmentSimulation.Tests
{
    /// <summary>
    /// Test class for Program functionality.
    /// Contains unit tests for temperature sensors, heaters and fans.
    /// </summary>
    public class ProgramTests
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramTests"/> class.
        /// Sets up the HTTP client with base address and API key.
        /// </summary>
        public ProgramTests()
        {
            _client = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5077/") };
            _client.DefaultRequestHeaders.Add("X-Api-Key", "API_KEY_CLIENT_3");
        }

        /// <summary>
        /// Tests that GetSensorTemperature returns a temperature value within a realistic range.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task GetSensorTemperature_ShouldReturnTemperature()
        {
            // Arrange
            int sensorId = 1;

            // Act
            double temperature = await Program.GetSensorTemperature(_client, sensorId);

            // Assert
            Assert.True(temperature >= -50 && temperature <= 50, "Temperature should be within a realistic range.");
        }

        /// <summary>
        /// Tests that GetSensorTemperature throws exception for invalid sensor ID.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task GetSensorTemperature_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            int invalidSensorId = -1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                Program.GetSensorTemperature(_client, invalidSensorId));
        }

        /// <summary>
        /// Tests that SetHeaterLevel successfully sets a heater level.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task SetHeaterLevel_ShouldSetLevel()
        {
            // Arrange
            int heaterId = 1;
            int level = 3;

            // Act
            await Program.SetHeaterLevel(_client, heaterId, level);

            // Assert
            // No exception means success
        }

        /// <summary>
        /// Tests that SetHeaterLevel throws exception for invalid heater ID.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task SetHeaterLevel_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            int invalidHeaterId = -1;
            int level = 3;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                Program.SetHeaterLevel(_client, invalidHeaterId, level));
        }

        /// <summary>
        /// Tests that SetHeaterLevel throws exception for invalid level value.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task SetHeaterLevel_WithInvalidLevel_ShouldThrowException()
        {
            // Arrange
            int heaterId = 1;
            int invalidLevel = 11; // Assuming valid levels are 0-10

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                Program.SetHeaterLevel(_client, heaterId, invalidLevel));
        }

        /// <summary>
        /// Tests that SetHeaterLevel works with minimum level.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task SetHeaterLevel_WithMinimumLevel_ShouldSetLevel()
        {
            // Arrange
            int heaterId = 1;
            int minLevel = 0;

            // Act
            await Program.SetHeaterLevel(_client, heaterId, minLevel);

            // Assert
            // No exception means success
        }

        /// <summary>
        /// Tests that SetHeaterLevel works with maximum level.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task SetHeaterLevel_WithMaximumLevel_ShouldSetLevel()
        {
            // Arrange
            int heaterId = 1;
            int maxLevel = 10; // Assuming valid levels are 0-10

            // Act
            await Program.SetHeaterLevel(_client, heaterId, maxLevel);

            // Assert
            // No exception means success
        }

        /// <summary>
        /// Tests that SetFanState successfully sets a fan state.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task SetFanState_ShouldSetState()
        {
            // Arrange
            int fanId = 1;
            bool isOn = true;

            // Act
            await Program.SetFanState(_client, fanId, isOn);

            // Assert
            // No exception means success
        }

        /// <summary>
        /// Tests that SetFanState throws exception for invalid fan ID.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task SetFanState_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            int invalidFanId = -1;
            bool isOn = true;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                Program.SetFanState(_client, invalidFanId, isOn));
        }

        /// <summary>
        /// Tests that SetFanState can toggle a fan off.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task SetFanState_TurnOff_ShouldSetState()
        {
            // Arrange
            int fanId = 1;
            bool isOn = false;

            // Act
            await Program.SetFanState(_client, fanId, isOn);

            // Assert
            // No exception means success
        }

        /// <summary>
        /// Tests that GetAverageTemperature returns an average temperature within a realistic range.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task GetAverageTemperature_ShouldReturnAverage()
        {
            // Act
            double averageTemperature = await Program.GetAverageTemperature(_client);

            // Assert
            Assert.True(averageTemperature >= -50 && averageTemperature <= 50, "Average temperature should be within a realistic range.");
        }
    }
}