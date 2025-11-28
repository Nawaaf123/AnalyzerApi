namespace AnalyzerApi.Models
{
    public class FileSnippet
    {
        public string FilePath { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
