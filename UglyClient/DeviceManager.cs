using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnvironmentSimulation
{
    public class DeviceManager
    {
        private readonly Dictionary<int, FanContext> _fans = new();
        private readonly Dictionary<int, HeaterContext> _heaters = new();
        private readonly HttpClient _client;

        public DeviceManager(HttpClient client, int numFans = 3, int numHeaters = 3)
        {
            _client = client;

            // Initialize with default states
            for (int i = 1; i <= numFans; i++)
            {
                _fans.Add(i, new FanContext(i, false));
            }

            for (int i = 1; i <= numHeaters; i++)
            {
                _heaters.Add(i, new HeaterContext(i, 0));
            }
        }

        public async Task InitializeFromServerAsync()
        {
            // Fetch all fan states
            for (int i = 1; i <= _fans.Count; i++)
            {
                var fanResponse = await _client.GetAsync($"api/fans/{i}/state");
                if (fanResponse.IsSuccessStatusCode)
                {
                    var fanJson = await fanResponse.Content.ReadAsStringAsync();
                    var fan = JsonSerializer.Deserialize<Program.FanDTO>(fanJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _fans[i].SetState(fan.IsOn ? new FanOnState(i) : new FanOffState(i));
                }
            }

            // Fetch all heater levels
            for (int i = 1; i <= _heaters.Count; i++)
            {
                var heaterResponse = await _client.GetAsync($"api/heat/{i}/level");
                if (heaterResponse.IsSuccessStatusCode)
                {
                    var levelString = await heaterResponse.Content.ReadAsStringAsync();
                    if (int.TryParse(levelString, out int level))
                    {
                        HeaterState state = level switch
                        {
                            0 => new HeaterOffState(i),
                            1 => new HeaterLowState(i),
                            2 => new HeaterMediumState(i),
                            3 => new HeaterHighState(i),
                            4 => new HeaterVeryHighState(i),
                            5 => new HeaterMaxState(i),
                            _ => new HeaterOffState(i)
                        };

                        _heaters[i].SetState(state);
                    }
                }
            }
        }

        public async Task<bool> ControlFanAsync(int fanId, bool turnOn)
        {
            if (!_fans.ContainsKey(fanId))
            {
                throw new ArgumentException($"Fan with ID {fanId} does not exist");
            }

            await _fans[fanId].SetSpecificStateAsync(_client, turnOn);
            return _fans[fanId].IsOn;
        }

        public async Task<int> ControlHeaterAsync(int heaterId, int level)
        {
            if (!_heaters.ContainsKey(heaterId))
            {
                throw new ArgumentException($"Heater with ID {heaterId} does not exist");
            }

            await _heaters[heaterId].SetSpecificLevelAsync(_client, level);
            return _heaters[heaterId].CurrentLevel;
        }

        public async Task SetAllFansAsync(bool turnOn)
        {
            foreach (var fanId in _fans.Keys)
            {
                await _fans[fanId].SetSpecificStateAsync(_client, turnOn);
            }
        }

        public async Task SetAllHeatersAsync(int level)
        {
            foreach (var heaterId in _heaters.Keys)
            {
                await _heaters[heaterId].SetSpecificLevelAsync(_client, level);
            }
        }

        public Dictionary<int, string> GetAllFanStates()
        {
            var states = new Dictionary<int, string>();
            foreach (var fan in _fans)
            {
                states.Add(fan.Key, $"Fan {fan.Key}: {fan.Value.CurrentStateName}");
            }
            return states;
        }

        public Dictionary<int, string> GetAllHeaterStates()
        {
            var states = new Dictionary<int, string>();
            foreach (var heater in _heaters)
            {
                states.Add(heater.Key, $"Heater {heater.Key}: {heater.Value.CurrentStateName}");
            }
            return states;
        }
    }
}