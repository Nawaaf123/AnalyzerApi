# 🔍 Analyzer API  
A powerful .NET 8 Web API that analyzes other APIs, reads source code files, checks health endpoints, extracts Swagger metadata, and generates a full AI-powered technical audit report — including optional PDF report generation.

This tool is designed for developers, DevOps engineers, and QA teams who want quick insights into the structure, quality, and health of any API.

---

## 🚀 Features

### **✔ API Structure Analysis**
- Reads any target API URL  
- Attempts to retrieve:
  - Swagger JSON  
  - Health endpoints  
  - Basic metadata  
- Detects missing or failing endpoints

### **✔ Code File Reader**
- Scans a local project folder  
- Extracts `.cs`, `.json`, `.sql`, `.csproj`, `.sln` files  
- Limits file count and max characters for safe AI analysis  
- Sends code snippets for review

### **✔ AI-Powered Analysis**
- Uses OpenAI (GPT-4o-mini or similar) to generate:
  - Architecture review  
  - Security audit  
  - Performance review  
  - Code quality assessment  
  - Missing best practices  
  - Overall score and recommendations  

### **✔ PDF Report Generation**
- Outputs a polished PDF containing:
  - Summary  
  - Full detailed audit report  
  - Files analyzed  
  - API metadata  
  - Health status  

### **✔ Fully REST-Based**
Exposes two endpoints:

#### **POST /api/analyzer/run**
Returns:
- JSON report  
- OpenAI summary  
- API metadata  
- Code analysis  

#### **POST /api/analyzer/run-pdf**
Returns:
- A downloadable PDF file with the complete report

---

## 🛠️ Technologies Used

- **.NET 8 Web API**
- **OpenAI API (Chat Completions)**
- **QuestPDF (PDF Generation)**
- **HttpClient** for metadata extraction
- **Dependency Injection**
- **Swagger / OpenAPI**
- **C# 12, modern async/await patterns**

---

## 📁 Project Structure

AnalyzerApi/
├── Controllers/
│ └── AnalyzerController.cs
│ └── WeatherForecastController.cs
├── Services/
│ ├── FileCollectorService.cs
│ ├── ApiMetadataService.cs
│ ├── ChatAnalysisService.cs
│ └── PdfReportService.cs
├── Models/
│ ├── AnalyzeRequest.cs
│ ├── AnalysisResult.cs
│ └── FileSnippet.cs
├── appsettings.json
├── Program.cs
└── README.md


---

## 📦 How It Works (Flow)

1. Client sends:
   - Target API Base URL  
   - Local folder path  
   - Max file limit  
   - Max character limit per file  

2. The API:
   - Reads project files  
   - Fetches API metadata  
   - Sends all content to OpenAI  
   - Receives detailed technical analysis  
   - Formats the result  
   - Optionally generates a PDF  

3. Client receives:
   - JSON response  
   - OR PDF download  

---

## ⚙️ Configuration

Add your OpenAI key in `appsettings.json` or environment variables:

```json
"OpenAI": {
  "ApiKey": "YOUR_API_KEY"
}


📤 PDF Output Preview

The PDF contains:

Summary

Full detailed report

Files analyzed

Swagger preview

Health endpoints

Recommendations

Scores & grades




🧠 Ideal Use Cases

API auditing

Code quality assessment

Pre-deployment checks

DevOps CI/CD integration

Automated documentation

Cross-team code reviews


🛡️ Disclaimer

No API keys or secrets should ever be committed to this repository.
Add your own keys through environment variables for security.

⭐ Contributions

Pull requests are welcome!
For significant changes, please open an issue first.

📜 License

This project is open-source and available under the MIT License.
