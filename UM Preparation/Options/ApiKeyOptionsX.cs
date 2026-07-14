namespace UM_Preparation.Options
{
    public class ApiKeyOptionsX
    {
        public const string AuthenticationApiKey = "Authentication:ApiKey";

        public required string HeaderName { get; set; }
        public required string Key { get; set; }
    }
}