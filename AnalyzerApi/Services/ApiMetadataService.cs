using System.Text;

namespace AnalyzerApi.Services
{
    public class ApiMetadataService
    {
        private readonly HttpClient _http;
        private readonly ILogger<ApiMetadataService> _logger;

        public ApiMetadataService(HttpClient http, ILogger<ApiMetadataService> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<(string? swagger, string? health)> CollectMetadataAsync(string? baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                return (null, "No baseUrl provided");

            baseUrl = baseUrl.TrimEnd('/');

            string? swaggerJson = null;
            var sb = new StringBuilder();

            // Try swagger.json
            try
            {
                swaggerJson = await _http.GetStringAsync($"{baseUrl}/swagger/v1/swagger.json");
                sb.AppendLine("Swagger JSON: OK");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Swagger JSON: FAILED ({ex.Message})");
                _logger.LogWarning(ex, "Failed to fetch Swagger from {BaseUrl}", baseUrl);
            }

            async Task Probe(string path, string label)
            {
                var url = $"{baseUrl}{path}";
                try
                {
                    var res = await _http.GetAsync(url);
                    sb.AppendLine($"{label} [{path}] → {(int)res.StatusCode} {res.StatusCode}");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"{label} [{path}] → FAILED ({ex.Message})");
                }
            }

            await Probe("/", "Root");
            await Probe("/swagger", "Swagger UI");
            await Probe("/swagger/index.html", "Swagger UI HTML");
            await Probe("/api/health", "Health (api/health)");
            await Probe("/health", "Health (/health)");
            await Probe("/api/status", "Status");
            await Probe("/api/ping", "Ping");

            return (swaggerJson, sb.ToString());
        }
    }
}
