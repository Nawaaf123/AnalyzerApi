using System.Text;
using AnalyzerApi.Models;
using OpenAI.Chat;

namespace AnalyzerApi.Services
{
    public class ChatAnalysisService
    {
        private readonly string _apiKey;
        private readonly ILogger<ChatAnalysisService> _logger;

        public ChatAnalysisService(IConfiguration config, ILogger<ChatAnalysisService> logger)
        {
            _apiKey = config["OpenAI:ApiKey"]
                ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new Exception("OpenAI API key not configured (OpenAI:ApiKey or ENV).");

            _logger = logger;
        }

        public async Task<AnalysisResult> AnalyzeAsync(
            string? baseUrl,
            List<FileSnippet> files,
            string? swagger,
            string? health)
        {
            // 🔥 SUPER SYSTEM PROMPT
            var systemPrompt = @"
You are an elite senior software architect, security engineer, and performance expert.

You will receive:
- API base URL
- Health/probe information
- Optional Swagger JSON
- Many source code files from an API project (any language: C#, JS/TS, Python, Java, PHP, Go, etc.)

Your job:
1. Detect CRITICAL BUGS that can prevent the API from running.
2. Perform an OWASP-style SECURITY AUDIT (SQL injection, missing auth, hardcoded secrets, CORS, HTTPS, etc.).
3. Review ARCHITECTURE (SOLID, clean layering, controllers/services, folder structure).
4. Analyze PERFORMANCE (async/await usage, DB queries, caching, pagination, memory).
5. Analyze CODE QUALITY (error handling, logging, duplication, readability).
6. Analyze each ENDPOINT (path, method, issues, improvements).
7. Analyze DEPENDENCIES (obvious outdated or insecure uses).
8. Provide AUTO-FIX SUGGESTIONS with improved code snippets.
9. Provide a FINAL UPGRADE ROADMAP.

Return the result as detailed MARKDOWN with clear headings:

# Summary
# Critical Errors
# Security Audit
# Architecture Review
# Performance Review
# Code Quality Review
# Endpoint-by-Endpoint Analysis
# Dependency & Framework Analysis
# Auto-Fix Suggestions (with code)
# Recommended Refactor / Roadmap
# Scores
- Security: X/100
- Architecture: X/100
- Performance: X/100
- Overall Grade: A–F
";

            var user = new StringBuilder();

            user.AppendLine("=== API CONTEXT ===");
            user.AppendLine($"Base URL: {baseUrl ?? "none"}");

            user.AppendLine("\n=== HEALTH / PROBE INFO ===");
            user.AppendLine(health ?? "No health info");

            user.AppendLine("\n=== SWAGGER JSON (trimmed) ===");
            if (!string.IsNullOrWhiteSpace(swagger))
            {
                var trimmed = swagger.Length > 4000 ? swagger[..4000] + "...(trimmed)" : swagger;
                user.AppendLine(trimmed);
            }
            else
            {
                user.AppendLine("No swagger available.");
            }

            user.AppendLine("\n=== SOURCE FILES (TRIMMED) ===");
            foreach (var f in files)
            {
                user.AppendLine($"\n--- FILE: {f.RelativePath} ---\n");
                user.AppendLine(f.Content);
            }

            try
            {
                // ✅ This matches your SDK: ChatClient(model, apiKey)
                var chat = new ChatClient("gpt-4o-mini", _apiKey);

                var messages = new List<ChatMessage>
                {
                    ChatMessage.CreateSystemMessage(systemPrompt),
                    ChatMessage.CreateUserMessage(user.ToString())
                };

                var response = await chat.CompleteChatAsync(messages);

                // ✅ Your SDK: use Content[0].Text (not Choices)
                string aiText = response.Value.Content[0].Text;

                return new AnalysisResult
                {
                    Summary = "Analysis completed successfully. See FullReport.",
                    FullReport = aiText,
                    FilesAnalyzed = files,
                    SwaggerJson = swagger,
                    HealthCheckStatus = health,
                    TargetApiBaseUrl = baseUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling OpenAI.");

                return new AnalysisResult
                {
                    Summary = "Analysis failed due to OpenAI error.",
                    FullReport = ex.Message,
                    FilesAnalyzed = files,
                    SwaggerJson = swagger,
                    HealthCheckStatus = health,
                    TargetApiBaseUrl = baseUrl
                };
            }
        }
    }
}
