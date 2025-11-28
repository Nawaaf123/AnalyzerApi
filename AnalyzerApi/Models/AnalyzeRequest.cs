namespace AnalyzerApi.Models
{
    public class AnalyzeRequest
    {
        // Base URL of the API we want to probe (for swagger/health/etc.)
        public string? TargetApiBaseUrl { get; set; }

        // Local folder path of the API source code to analyze
        public string? LocalFolderPath { get; set; }

        // Optional overrides (otherwise defaults from config)
        public int? MaxFiles { get; set; }
        public int? MaxFileCharacters { get; set; }
    }
}
