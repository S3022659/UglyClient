namespace EnvironmentSimulation.Configuration
{
    public class EnvironmentConfig
    {
        public required string ApiBaseUrl { get; set; }
        public required string ApiKey { get; set; }
        public int NumSensors { get; set; }
        public int NumHeaters { get; set; }
        public int NumFans { get; set; }
    }
}
