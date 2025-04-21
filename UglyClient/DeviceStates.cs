using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EnvironmentSimulation
{
    // Base state interface for any device
    public interface IDeviceState
    {
        string GetStateName();
        Task<IDeviceState> TransitionAsync(HttpClient client);
    }

    // Fan states
    public abstract class FanState : IDeviceState
    {
        protected readonly int _fanId;

        public FanState(int fanId)
        {
            _fanId = fanId;
        }

        public abstract string GetStateName();
        public abstract Task<IDeviceState> TransitionAsync(HttpClient client);
    }

    public class FanOnState : FanState
    {
        public FanOnState(int fanId) : base(fanId) { }

        public override string GetStateName() => "On";

        public override async Task<IDeviceState> TransitionAsync(HttpClient client)
        {
            await Program.SetFanState(client, _fanId, false);
            return new FanOffState(_fanId);
        }
    }

    public class FanOffState : FanState
    {
        public FanOffState(int fanId) : base(fanId) { }

        public override string GetStateName() => "Off";

        public override async Task<IDeviceState> TransitionAsync(HttpClient client)
        {
            await Program.SetFanState(client, _fanId, true);
            return new FanOnState(_fanId);
        }
    }

    // Heater states
    public abstract class HeaterState : IDeviceState
    {
        protected readonly int _heaterId;
        protected readonly int _currentLevel;

        public HeaterState(int heaterId, int currentLevel)
        {
            _heaterId = heaterId;
            _currentLevel = currentLevel;
        }

        public abstract string GetStateName();
        public abstract Task<IDeviceState> TransitionAsync(HttpClient client);

        public int GetLevel() => _currentLevel;
    }

    public class HeaterOffState : HeaterState
    {
        public HeaterOffState(int heaterId) : base(heaterId, 0) { }

        public override string GetStateName() => "Off (Level 0)";

        public override async Task<IDeviceState> TransitionAsync(HttpClient client)
        {
            await Program.SetHeaterLevel(client, _heaterId, 1);
            return new HeaterLowState(_heaterId);
        }
    }

    public class HeaterLowState : HeaterState
    {
        public HeaterLowState(int heaterId) : base(heaterId, 1) { }

        public override string GetStateName() => "Low (Level 1)";

        public override async Task<IDeviceState> TransitionAsync(HttpClient client)
        {
            await Program.SetHeaterLevel(client, _heaterId, 2);
            return new HeaterMediumState(_heaterId);
        }
    }

    public class HeaterMediumState : HeaterState
    {
        public HeaterMediumState(int heaterId) : base(heaterId, 2) { }

        public override string GetStateName() => "Medium (Level 2)";

        public override async Task<IDeviceState> TransitionAsync(HttpClient client)
        {
            await Program.SetHeaterLevel(client, _heaterId, 3);
            return new HeaterHighState(_heaterId);
        }
    }

    public class HeaterHighState : HeaterState
    {
        public HeaterHighState(int heaterId) : base(heaterId, 3) { }

        public override string GetStateName() => "High (Level 3)";

        public override async Task<IDeviceState> TransitionAsync(HttpClient client)
        {
            await Program.SetHeaterLevel(client, _heaterId, 4);
            return new HeaterVeryHighState(_heaterId);
        }
    }

    public class HeaterVeryHighState : HeaterState
    {
        public HeaterVeryHighState(int heaterId) : base(heaterId, 4) { }

        public override string GetStateName() => "Very High (Level 4)";

        public override async Task<IDeviceState> TransitionAsync(HttpClient client)
        {
            await Program.SetHeaterLevel(client, _heaterId, 5);
            return new HeaterMaxState(_heaterId);
        }
    }

    public class HeaterMaxState : HeaterState
    {
        public HeaterMaxState(int heaterId) : base(heaterId, 5) { }

        public override string GetStateName() => "Maximum (Level 5)";

        public override async Task<IDeviceState> TransitionAsync(HttpClient client)
        {
            await Program.SetHeaterLevel(client, _heaterId, 0);
            return new HeaterOffState(_heaterId);
        }
    }
}