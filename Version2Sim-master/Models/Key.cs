namespace Version2.Models
{
    public class ApiKeyManager
    {
        // Simulated storage for API keys
        private static readonly Dictionary<string, string> _apiKeys = new()
    {
        
        { "client3", "API_KEY_CLIENT_3" }
    };

        public bool ValidateApiKey(string apiKey)
        {
            return _apiKeys.ContainsValue(apiKey);
        }

        public string GetClientId(string apiKey)
        {
            return _apiKeys.FirstOrDefault(k => k.Value == apiKey).Key;
        }
    }

}
