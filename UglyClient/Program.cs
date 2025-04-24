using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnvironmentSimulation
{
    /// <summary>
    /// Main program class for the environment simulation client application.
    /// Provides functionality to interact with the environment simulation API.
    /// </summary>
    public class Program
    {
        private const int NUM_SENSORS = 3;
        private const int NUM_HEATERS = 3;
        private const int NUM_FANS = 3;

        private static DeviceManager? deviceManager;

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            // https://envrosym.azurewebsites.net/
            var client = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5077/") };
            // var client = new HttpClient { BaseAddress = new Uri("https://envrosym.azurewebsites.net/") };
            const string apiKey = "API_KEY_CLIENT_3"; // Replace with your actual API key

            // Add the API key to the default request headers
            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            deviceManager = new DeviceManager(client, NUM_FANS, NUM_HEATERS);
            await deviceManager.InitializeFromServerAsync();
            Console.WriteLine("Device states initialized from server.");

            while (true)
            {
                Console.WriteLine("Simulation Control:");
                Console.WriteLine("1. Control Fan");
                Console.WriteLine("2. Control Heater");
                Console.WriteLine("3. Read Temperature");
                Console.WriteLine("4. Display State of All Devices");
                Console.WriteLine("5. Control Simulation");
                Console.WriteLine("6. Reset Simulation");
                Console.Write("Select an option: ");

                // Read input option
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.Write("Enter Fan Number: ");
                        if (int.TryParse(Console.ReadLine(), out int fanId))
                        {
                            Console.Write("Turn Fan On or Off? (on/off): ");
                            var stateInput = Console.ReadLine();
                            bool isOn = stateInput?.ToLower() == "on";

                            try
                            {
                                bool newState = await deviceManager.ControlFanAsync(fanId, isOn);
                                Console.WriteLine($"Fan {fanId} has been turned {(newState ? "On" : "Off")}.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Fan Number.");
                        }
                        break;
                    case "2":
                        Console.Write("Enter Heater Number: ");
                        if (int.TryParse(Console.ReadLine(), out int heaterId))
                        {
                            Console.Write("Set Heater Level (0-5): ");
                            if (int.TryParse(Console.ReadLine(), out int level) && level >= 0 && level <= 5)
                            {
                                try
                                {
                                    int newLevel = await deviceManager.ControlHeaterAsync(heaterId, level);
                                    Console.WriteLine($"Heater {heaterId} level set to {newLevel}.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error: {ex.Message}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid Heater Level. Please enter a value between 0 and 5.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Heater Number.");
                        }
                        break;
                    case "3":

                        Console.Write("Enter Sensor Number: ");
                        if (int.TryParse(Console.ReadLine(), out int sensorId))
                        {
                            try
                            {
                                double temperature = await GetSensorTemperature(client, sensorId);
                                Console.WriteLine($"Sensor {sensorId} Temperature: {temperature:F1}°C");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Sensor Number.");
                        }
                        break;
                    case "4":
                        Console.WriteLine("Fetching the state of all devices...");

                        try
                        {
                            // Display fan states using our device manager
                            Console.WriteLine("Fan states:");
                            foreach (var fanState in deviceManager.GetAllFanStates())
                            {
                                Console.WriteLine($"  {fanState.Value}");
                            }

                            // Display heater states using our device manager
                            Console.WriteLine("Heater states:");
                            foreach (var heaterState in deviceManager.GetAllHeaterStates())
                            {
                                Console.WriteLine($"  {heaterState.Value}");
                            }

                            // Display sensor temperatures
                            Console.WriteLine("Sensor temperatures:");
                            await DisplayAllSensorTemperatures(client);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error fetching device states: {ex.Message}");
                        }
                        break;
                    case "5":
                        //await RunTemperatureControlLoop(client);
                        Console.WriteLine("Starting temperature control algorithm...");
                        Console.Write("Provide a final Temp Value: ");
                        double currentTemperature = await GetAverageTemperature(client);
                        while (true)
                        {
                            // Phase 1: Gradually increase to 20°C over 30 seconds
                            currentTemperature = await AdjustTemperature(client, currentTemperature, 20.0, 30);

                            // Phase 2: Rapidly cool to 16°C
                            currentTemperature = await AdjustTemperature(client, currentTemperature, 16.0, 10);

                            // Phase 3: Hold at 16°C for 10 seconds
                            currentTemperature = await HoldTemperature(client, currentTemperature, 16.0, 10);

                            // Phase 4: Gradually return to 18°C and maintain
                            currentTemperature = await AdjustTemperature(client, currentTemperature, 18.0, 20);
                            currentTemperature = await HoldTemperature(client, currentTemperature, 18.0, int.MaxValue); // Maintain until exit
                        }
                    case "6":
                        // await Reset(client);
                        Console.WriteLine("Resetting client state...");

                        try
                        {
                            // Send a POST request to the reset endpoint
                            var response = await client.PostAsync("api/Envo/reset", null);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Client state has been successfully reset.");
                                Console.WriteLine("Fetching the state of all devices...");

                                try
                                {
                                    // Get individual fan states
                                    Console.WriteLine("Fetching fan states individually...");
                                    for (int i = 1; i <= 3; i++) // Assuming there are 3 fans for this example
                                    {
                                        var fanResponse = await client.GetAsync($"api/fans/{i}/state");
                                        if (fanResponse.IsSuccessStatusCode)
                                        {
                                            var fanJson = await fanResponse.Content.ReadAsStringAsync();
                                            var fan = JsonSerializer.Deserialize<FanDTO>(fanJson, new JsonSerializerOptions
                                            {
                                                PropertyNameCaseInsensitive = true
                                            });
                                            if (fan != null)
                                            {
                                                Console.WriteLine($"  Fan {fan.Id}: {(fan.IsOn ? "On" : "Off")}");
                                            }
                                            else
                                            {
                                                Console.WriteLine("  Fan data is null or could not be deserialized.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"  Fan {i}: Failed to fetch state.");
                                        }
                                    }

                                    // Get individual heater levels
                                    Console.WriteLine("Fetching heater levels individually...");
                                    for (int i = 1; i <= 3; i++) // Assuming there are 3 heaters for this example
                                    {
                                        var heaterResponse = await client.GetAsync($"api/heat/{i}/level");
                                        if (heaterResponse.IsSuccessStatusCode)
                                        {
                                            var levelString = await heaterResponse.Content.ReadAsStringAsync();
                                            if (int.TryParse(levelString, out int level))
                                            {
                                                Console.WriteLine($"  Heater {i}: Level {level}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"  Heater {i}: Failed to parse level.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"  Heater {i}: Failed to fetch level.");
                                        }
                                    }

                                    // Get individual sensor temperatures
                                    Console.WriteLine("Fetching sensor temperatures individually...");
                                    await DisplayAllSensorTemperatures(client);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error fetching device states: {ex.Message}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Failed to reset client state: {response.ReasonPhrase}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error while resetting client state: {ex.Message}");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine(); // Add a blank line for better readability
            }
        }

        /// <summary>
        /// Resets the system to its initial state.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task Reset(HttpClient client)
        {
            Console.WriteLine("Resetting client state...");

            try
            {
                // Send a POST request to the reset endpoint
                var response = await client.PostAsync("api/Envo/reset", null);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Client state has been successfully reset.");
                    Console.WriteLine("Fetching the state of all devices...");

                    try
                    {
                        // Get individual fan states
                        Console.WriteLine("Fetching fan states individually...");
                        for (int i = 1; i <= 3; i++) // Assuming there are 3 fans for this example
                        {
                            var fanResponse = await client.GetAsync($"api/fans/{i}/state");
                            if (fanResponse.IsSuccessStatusCode)
                            {
                                var fanJson = await fanResponse.Content.ReadAsStringAsync();
                                var fan = JsonSerializer.Deserialize<FanDTO>(fanJson, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                if (fan != null)
                                {
                                    Console.WriteLine($"  Fan {fan.Id}: {(fan.IsOn ? "On" : "Off")}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"  Fan {i}: Failed to fetch state.");
                            }
                        }

                        // Get individual heater levels
                        Console.WriteLine("Fetching heater levels individually...");
                        for (int i = 1; i <= 3; i++) // Assuming there are 3 heaters for this example
                        {
                            var heaterResponse = await client.GetAsync($"api/heat/{i}/level");
                            if (heaterResponse.IsSuccessStatusCode)
                            {
                                var levelString = await heaterResponse.Content.ReadAsStringAsync();
                                if (int.TryParse(levelString, out int level))
                                {
                                    Console.WriteLine($"  Heater {i}: Level {level}");
                                }
                                else
                                {
                                    Console.WriteLine($"  Heater {i}: Failed to parse level.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"  Heater {i}: Failed to fetch level.");
                            }
                        }

                        // Get individual sensor temperatures
                        Console.WriteLine("Fetching sensor temperatures individually...");
                        await DisplayAllSensorTemperatures(client);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching device states: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to reset client state: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while resetting client state: {ex.Message}");
            }
        }

        /// <summary>
        /// Runs the temperature control simulation loop according to a predefined pattern.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task RunTemperatureControlLoop(HttpClient client)
        {
            Console.WriteLine("Starting temperature control algorithm...");

            double currentTemperature = await GetAverageTemperature(client);

            // Prompt user for the final target temperature in Phase 4
            Console.Write("Enter the final target temperature for Phase 4: ");
            if (!double.TryParse(Console.ReadLine(), out double finalTargetTemperature))
            {
                Console.WriteLine("Invalid input. Please enter a valid numeric temperature.");
                return;
            }

            while (true)
            {
                // Phase 1: Gradually increase to 20°C over 30 seconds
                currentTemperature = await AdjustTemperature(client, currentTemperature, 20.0, 30);

                // Phase 2: Rapidly cool to 16°C
                currentTemperature = await AdjustTemperature(client, currentTemperature, 16.0, 10);

                // Phase 3: Hold at 16°C for 10 seconds
                currentTemperature = await HoldTemperature(client, currentTemperature, 16.0, 10);

                // Phase 4: Gradually adjust to the user-defined target temperature and maintain
                currentTemperature = await AdjustTemperature(client, currentTemperature, finalTargetTemperature, 20);
                currentTemperature = await HoldTemperature(client, currentTemperature, finalTargetTemperature, int.MaxValue); // Maintain until exit
            }
        }

        /// <summary>
        /// Gradually adjusts the temperature to a target value over a specified duration.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <param name="currentTemperature">The current temperature.</param>
        /// <param name="targetTemperature">The target temperature to reach.</param>
        /// <param name="durationSeconds">The duration in seconds over which to adjust the temperature.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the new temperature.</returns>
        public static async Task<double> AdjustTemperature(HttpClient client, double currentTemperature, double targetTemperature, int durationSeconds)
        {
            Console.WriteLine($"Adjusting temperature to {targetTemperature}°C over {durationSeconds} seconds...");
            int intervalMs = 1000; // 1-second intervals
            int iterations = durationSeconds;

            for (int i = 0; i < iterations; i++)
            {
                if (Math.Abs(currentTemperature - targetTemperature) <= 0.1) break;

                if (currentTemperature < targetTemperature)
                {
                    // Turn on heaters and reduce fan activity
                    await SetAllHeaters(client, 3); // Set heaters to level 3
                    await SetAllFans(client, false); // Turn off fans
                }
                else
                {
                    // Turn off heaters and increase fan activity
                    await SetAllHeaters(client, 0); // Turn off heaters
                    await SetAllFans(client, true); // Turn on fans
                }

                // Wait for a second and fetch the updated temperature
                await Task.Delay(intervalMs);
                currentTemperature = await GetAverageTemperature(client);
                Console.WriteLine($"Current Temperature: {currentTemperature:F1}°C");
            }

            return currentTemperature;
        }

        /// <summary>
        /// Attempts to maintain a specific temperature for a specified duration.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <param name="currentTemperature">The current temperature.</param>
        /// <param name="targetTemperature">The temperature to maintain.</param>
        /// <param name="durationSeconds">The duration in seconds to maintain the temperature.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the final temperature.</returns>
        public static async Task<double> HoldTemperature(HttpClient client, double currentTemperature, double targetTemperature, int durationSeconds)
        {
            Console.WriteLine($"Holding temperature at {targetTemperature}°C for {durationSeconds} seconds...");
            int intervalMs = 1000; // 1-second intervals

            for (int i = 0; i < durationSeconds; i++)
            {
                if (currentTemperature < targetTemperature)
                {
                    // Turn on heaters slightly and reduce fans
                    await SetAllHeaters(client, 1); // Minimal heating
                    await SetAllFans(client, false); // Reduce cooling
                }
                else if (currentTemperature > targetTemperature)
                {
                    // Turn off heaters and increase fans
                    await SetAllHeaters(client, 0); // Turn off heating
                    await SetAllFans(client, true); // Activate cooling
                }

                // Wait for a second and fetch the updated temperature
                await Task.Delay(intervalMs);
                currentTemperature = await GetAverageTemperature(client);
                Console.WriteLine($"Current Temperature: {currentTemperature:F1}°C");
            }

            return currentTemperature;
        }

        /// <summary>
        /// Calculates the average temperature across all sensors.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the average temperature.</returns>
        public static async Task<double> GetAverageTemperature(HttpClient client)
        {
            double totalTemperature = 0;
            for (int i = 1; i <= NUM_SENSORS; i++)
            {
                totalTemperature += await GetSensorTemperature(client, i);
            }
            return totalTemperature / NUM_SENSORS;
        }

        /// <summary>
        /// Sets all heaters to the specified level.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <param name="level">The heater level to set (0-5).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task SetAllHeaters(HttpClient client, int level)
        {
            if (deviceManager != null)
            {
                await deviceManager.SetAllHeatersAsync(level);
            }
            else
            {
                Console.WriteLine("Error: Device manager is not initialized.");
            }
        }

        /// <summary>
        /// Sets all fans to the specified state.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <param name="state">The state to set (true for on, false for off).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task SetAllFans(HttpClient client, bool state)
        {
            if (deviceManager != null)
            {
                await deviceManager.SetAllFansAsync(state);
            }
            else
            {
                Console.WriteLine("Error: Device manager is not initialized.");
            }
        }

        /// <summary>
        /// Gets the temperature reading from a specific sensor.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <param name="sensorId">The ID of the sensor to read.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the temperature in degrees Celsius.</returns>
        public static async Task<double> GetSensorTemperature(HttpClient client, int sensorId)
        {
            var response = await client.GetAsync($"api/sensor/{sensorId}");
            if (response.IsSuccessStatusCode)
            {
                var tempString = await response.Content.ReadAsStringAsync();
                return double.Parse(tempString);
            }

            throw new Exception($"Failed to get temperature from sensor {sensorId}: {response.ReasonPhrase}");
        }

        /// <summary>
        /// Sets the power level of a specific heater.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <param name="heaterId">The ID of the heater to control.</param>
        /// <param name="level">The power level to set (0-5).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SetHeaterLevel(HttpClient client, int heaterId, int level)
        {
            var response = await client.PostAsync($"api/heat/{heaterId}",
                new StringContent(level.ToString(), System.Text.Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to set heater level {heaterId}: {response.ReasonPhrase}");
            }
        }

        /// <summary>
        /// Sets the state of a specific fan.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <param name="fanId">The ID of the fan to control.</param>
        /// <param name="isOn">The state to set (true for on, false for off).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SetFanState(HttpClient client, int fanId, bool isOn)
        {
            var response = await client.PostAsync($"api/fans/{fanId}",
                new StringContent(isOn.ToString().ToLower(), System.Text.Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to set fan state for fan {fanId}: {response.ReasonPhrase}");
            }
        }

        /// <summary>
        /// Displays the temperature readings from all sensors.
        /// </summary>
        /// <param name="client">The HTTP client to use for API communication.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task DisplayAllSensorTemperatures(HttpClient client)
        {
            Console.WriteLine("Fetching sensor temperatures individually...");
            for (int i = 1; i <= NUM_SENSORS; i++)
            {
                try
                {
                    var temp = await GetSensorTemperature(client, i);
                    Console.WriteLine($"  Sensor {i}: Temperature {temp:F1} (Deg)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Sensor {i}: Failed to read temperature - {ex.Message}");
                }
            }
        }

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

    /// <summary>
    /// Interface for environment services.
    /// </summary>
    public interface IEnvironmentService
    {
        /// <summary>
        /// Gets the temperature from a specific sensor.
        /// </summary>
        /// <param name="sensorId">The ID of the sensor.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the temperature.</returns>
        Task<double> GetTemperature(int sensorId);

        /// <summary>
        /// Sets the state of a specific fan.
        /// </summary>
        /// <param name="fanId">The ID of the fan.</param>
        /// <param name="state">The state to set (true for on, false for off).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetFanState(int fanId, bool state);

        /// <summary>
        /// Sets the level of a specific heater.
        /// </summary>
        /// <param name="heaterId">The ID of the heater.</param>
        /// <param name="level">The level to set (0-5).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetHeaterLevel(int heaterId, int level);
    }

    /// <summary>
    /// Enumeration for device types.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// Represents a sensor device.
        /// </summary>
        Sensor,

        /// <summary>
        /// Represents a fan device.
        /// </summary>
        Fan,

        /// <summary>
        /// Represents a heater device.
        /// </summary>
        Heater
    }

    /// <summary>
    /// Represents a device in the environment simulation.
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

    /// <summary>
    /// Represents a temperature reading.
    /// </summary>
    public class Temperature
    {
        /// <summary>
        /// Gets or sets the temperature value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the unit of the temperature.
        /// </summary>
        public required string Unit { get; set; }
    }
}
