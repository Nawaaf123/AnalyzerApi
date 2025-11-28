namespace AnalyzerApi.Models
{
    public class AnalysisResult
    {
        public string Summary { get; set; } = string.Empty;

        // This will contain the BIG JSON/Markdown report from ChatGPT
        public string FullReport { get; set; } = string.Empty;

        public List<FileSnippet> FilesAnalyzed { get; set; } = new();
        public string? SwaggerJson { get; set; }
        public string? HealthCheckStatus { get; set; }
        public string? TargetApiBaseUrl { get; set; }
    }
}
