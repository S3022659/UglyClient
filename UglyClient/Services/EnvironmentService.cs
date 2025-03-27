using System.Net.Http;
using System.Threading.Tasks;
using EnvironmentSimulation.Models;

namespace EnvironmentSimulation.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly HttpClient _client;

        public EnvironmentService(HttpClient client)
        {
            _client = client;
        }

        public async Task<double> GetTemperature(int sensorId)
        {
            var response = await _client.GetAsync($"api/sensor/{sensorId}");
            response.EnsureSuccessStatusCode();
            var tempString = await response.Content.ReadAsStringAsync();
            return double.Parse(tempString);
        }

        public async Task SetFanState(int fanId, bool state)
        {
            var response = await _client.PostAsync($"api/fans/{fanId}",
                new StringContent(state.ToString().ToLower(), System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }

        public async Task SetHeaterLevel(int heaterId, int level)
        {
            var response = await _client.PostAsync($"api/heat/{heaterId}",
                new StringContent(level.ToString(), System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }
    }
}
