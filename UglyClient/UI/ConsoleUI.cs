using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EnvironmentSimulation.UI
{
    public class ConsoleUI
    {
        private readonly HttpClient _client;

        public ConsoleUI(HttpClient client)
        {
            _client = client;
        }

        public async Task Run()
        {
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

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await ControlFan();
                        break;
                    case "2":
                        await ControlHeater();
                        break;
                    case "3":
                        await ReadTemperature();
                        break;
                    case "4":
                        await DisplayStateOfAllDevices();
                        break;
                    case "5":
                        await ControlSimulation();
                        break;
                    case "6":
                        await ResetSimulation();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine(); // Add a blank line for better readability
            }
        }

        private async Task ControlFan()
        {
            Console.Write("Enter Fan Number: ");
            if (int.TryParse(Console.ReadLine(), out int fanId))
            {
                Console.Write("Turn Fan On or Off? (on/off): ");
                var stateInput = Console.ReadLine();
                bool isOn = stateInput?.ToLower() == "on";

                try
                {
                    await Program.SetFanState(_client, fanId, isOn);
                    Console.WriteLine($"Fan {fanId} has been turned {(isOn ? "On" : "Off")}.");
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
        }

        private async Task ControlHeater()
        {
            Console.Write("Enter Heater Number: ");
            if (int.TryParse(Console.ReadLine(), out int heaterId))
            {
                Console.Write("Set Heater Level (0-5): ");
                if (int.TryParse(Console.ReadLine(), out int level) && level >= 0 && level <= 5)
                {
                    try
                    {
                        await Program.SetHeaterLevel(_client, heaterId, level);
                        Console.WriteLine($"Heater {heaterId} level set to {level}.");
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
        }

        private async Task ReadTemperature()
        {
            Console.Write("Enter Sensor Number: ");
            if (int.TryParse(Console.ReadLine(), out int sensorId))
            {
                try
                {
                    double temperature = await Program.GetSensorTemperature(_client, sensorId);
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
        }

        private async Task DisplayStateOfAllDevices()
        {
            Console.WriteLine("Fetching the state of all devices...");

            try
            {
                await Program.DisplayAllSensorTemperatures(_client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching device states: {ex.Message}");
            }
        }

        private async Task ControlSimulation()
        {
            Console.WriteLine("Starting temperature control algorithm...");
            double currentTemperature = await Program.GetAverageTemperature(_client);
            while (true)
            {
                currentTemperature = await Program.AdjustTemperature(_client, currentTemperature, 20.0, 30);
                currentTemperature = await Program.AdjustTemperature(_client, currentTemperature, 16.0, 10);
                currentTemperature = await Program.HoldTemperature(_client, currentTemperature, 16.0, 10);
                currentTemperature = await Program.AdjustTemperature(_client, currentTemperature, 18.0, 20);
                currentTemperature = await Program.HoldTemperature(_client, currentTemperature, 18.0, int.MaxValue);
            }
        }

        private async Task ResetSimulation()
        {
            Console.WriteLine("Resetting client state...");

            try
            {
                var response = await _client.PostAsync("api/Envo/reset", null);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Client state has been successfully reset.");
                    await DisplayStateOfAllDevices();
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
    }
}
