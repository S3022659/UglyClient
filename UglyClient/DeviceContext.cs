using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EnvironmentSimulation
{
    // Base context class for all devices
    public abstract class DeviceContext(int deviceId)
    {
        protected IDeviceState? _currentState;
        protected readonly int _deviceId = deviceId;

        public int DeviceId => _deviceId;

        public string CurrentStateName => _currentState?.GetStateName() ?? "Unknown";

        public async Task TransitionStateAsync(HttpClient client)
        {
            if (_currentState == null)
            {
                throw new InvalidOperationException("Current state is null and cannot transition.");
            }
            _currentState = await _currentState.TransitionAsync(client);
        }

        // Set specific state without transition
        public void SetState(IDeviceState state)
        {
            _currentState = state;
        }
    }

    // Fan context
    public class FanContext : DeviceContext
    {
        public FanContext(int fanId, bool initialIsOn) : base(fanId)
        {
            _currentState = initialIsOn ? (IDeviceState)new FanOnState(fanId) : new FanOffState(fanId);
        }

        public bool IsOn => _currentState is FanOnState;

        public async Task SetSpecificStateAsync(HttpClient client, bool turnOn)
        {
            if ((IsOn && !turnOn) || (!IsOn && turnOn))
            {
                await TransitionStateAsync(client);
            }
        }
    }

    // Heater context
    public class HeaterContext : DeviceContext
    {
        public HeaterContext(int heaterId, int initialLevel) : base(heaterId)
        {
            SetLevelState(initialLevel);
        }

        public int CurrentLevel => (_currentState as HeaterState)?.GetLevel() ?? 0;

        private void SetLevelState(int level)
        {
            _currentState = level switch
            {
                0 => new HeaterOffState(_deviceId),
                1 => new HeaterLowState(_deviceId),
                2 => new HeaterMediumState(_deviceId),
                3 => new HeaterHighState(_deviceId),
                4 => new HeaterVeryHighState(_deviceId),
                5 => new HeaterMaxState(_deviceId),
                _ => new HeaterOffState(_deviceId)
            };
        }

        public async Task SetSpecificLevelAsync(HttpClient client, int targetLevel)
        {
            if (targetLevel < 0 || targetLevel > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(targetLevel), "Heater level must be between 0 and 5");
            }

            await Program.SetHeaterLevel(client, _deviceId, targetLevel);
            SetLevelState(targetLevel);
        }
    }
}