using AnalyzerApi.Services;
using OpenAI;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;   // ? REQUIRED FOR FREE PDF USAGE

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register OpenAI Client
builder.Services.AddSingleton(sp =>
{
    var key = builder.Configuration["OpenAI:ApiKey"]
        ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

    return new OpenAIClient(key);
});

// Register Services
builder.Services.AddScoped<FileCollectorService>();
builder.Services.AddScoped<ChatAnalysisService>();
builder.Services.AddScoped<PdfReportService>();
builder.Services.AddHttpClient<ApiMetadataService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
