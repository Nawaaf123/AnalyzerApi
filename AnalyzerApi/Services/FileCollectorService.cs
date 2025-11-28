using AnalyzerApi.Models;

namespace AnalyzerApi.Services
{
    public class FileCollectorService
    {
        private readonly ILogger<FileCollectorService> _logger;
        private readonly IConfiguration _config;

        private static readonly string[] AllowedExtensions =
        {
            ".cs", ".csproj", ".sln", ".json", ".config", ".http", ".md", ".sql",
            ".js", ".ts", ".py", ".php", ".java", ".go", ".yml", ".yaml", ".dockerfile"
        };

        private static readonly string[] ExcludedFolders =
        {
            "bin", "obj", ".git", ".vs", "node_modules", "dist"
        };

        public FileCollectorService(ILogger<FileCollectorService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public List<FileSnippet> CollectFiles(string? folderPath, int? maxFilesOverride, int? maxCharsOverride)
        {
            var result = new List<FileSnippet>();

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                _logger.LogWarning("CollectFiles: folderPath invalid or missing: {Path}", folderPath);
                return result;
            }

            int maxFiles = maxFilesOverride ?? _config.GetValue("Analysis:MaxFiles", 40);
            int maxChars = maxCharsOverride ?? _config.GetValue("Analysis:MaxFileCharacters", 8000);

            _logger.LogInformation("Collecting files from {Path}. MaxFiles={MaxFiles}, MaxChars={MaxChars}",
                folderPath, maxFiles, maxChars);

            var allFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(f => AllowedExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                .Where(f => !IsInExcludedFolder(folderPath, f))
                .Take(maxFiles);

            foreach (var file in allFiles)
            {
                try
                {
                    var content = File.ReadAllText(file);

                    if (content.Length > maxChars)
                        content = content[..maxChars];

                    result.Add(new FileSnippet
                    {
                        FilePath = file,
                        RelativePath = Path.GetRelativePath(folderPath, file),
                        Content = content
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading file {File}", file);
                }
            }

            _logger.LogInformation("Collected {Count} files.", result.Count);
            return result;
        }

        private static bool IsInExcludedFolder(string root, string filePath)
        {
            var rel = Path.GetRelativePath(root, filePath);
            var segments = rel.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return segments.Any(seg =>
                ExcludedFolders.Contains(seg, StringComparer.OrdinalIgnoreCase));
        }
    }
}
