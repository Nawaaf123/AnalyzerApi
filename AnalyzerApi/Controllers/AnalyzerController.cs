using AnalyzerApi.Models;
using AnalyzerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalyzerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyzerController : ControllerBase
    {
        private readonly FileCollectorService _files;
        private readonly ApiMetadataService _metadata;
        private readonly ChatAnalysisService _chat;
        private readonly PdfReportService _pdf;

        public AnalyzerController(
            FileCollectorService files,
            ApiMetadataService metadata,
            ChatAnalysisService chat,
            PdfReportService pdf)
        {
            _files = files;
            _metadata = metadata;
            _chat = chat;
            _pdf = pdf;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run([FromBody] AnalyzeRequest req)
        {
            var collectedFiles = _files.CollectFiles(req.LocalFolderPath, req.MaxFiles, req.MaxFileCharacters);

            var (swagger, health) = await _metadata.CollectMetadataAsync(req.TargetApiBaseUrl);

            var result = await _chat.AnalyzeAsync(req.TargetApiBaseUrl, collectedFiles, swagger, health);

            return Ok(result);
        }

        [HttpPost("run-pdf")]
        public async Task<IActionResult> RunPdf([FromBody] AnalyzeRequest req)
        {
            var collectedFiles = _files.CollectFiles(req.LocalFolderPath, req.MaxFiles, req.MaxFileCharacters);

            var (swagger, health) = await _metadata.CollectMetadataAsync(req.TargetApiBaseUrl);

            var analysisResult = await _chat.AnalyzeAsync(req.TargetApiBaseUrl, collectedFiles, swagger, health);

            var pdfBytes = _pdf.GeneratePdf(analysisResult);

            return File(pdfBytes, "application/pdf", "ApiAnalysisReport.pdf");
        }
    }
}
