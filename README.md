# 🧠 AI SQL Assistant (Text-to-SQL Engine)

A high-performance **ASP.NET Core Web API** that integrates **Google Gemini** to convert natural language questions into safe SQL queries. This system executes queries against a **SQLite** database and returns dynamic result sets, allowing users to "chat" with their data.

This project implements a robust **"Escape Hatch" architecture**, utilizing a custom `HttpClient` wrapper to bypass SDK versioning conflicts and ensure stable access to the latest Gemini models (e.g., Flash 1.5/2.5).

---

## 🚀 Key Features

* **🗣️ Natural Language Querying:** Users can ask questions like *"What are the top 3 items with low stock?"* and get immediate data responses.
* **🧩 Dynamic Schema Injection:** The system automatically scans your C# Data Models for `[TableDescription]` and `[ColumnDescription]` attributes using Reflection. It builds the AI's "context map" at runtime, eliminating the need to manually update prompts when the database changes.
* **🛡️ Safety Guardrails:** Strictly enforces Read-Only access. The AI is sandboxed to generate only `SELECT` statements; `INSERT`, `UPDATE`, and `DELETE` attempts are blocked before execution.
* **⚡ High Performance:** Uses **Dapper** for high-speed dynamic query mapping and **Google Gemini Flash** for sub-second inference latency.
* **🔌 Direct API Integration:** Uses a custom implementation to communicate with Google's `v1beta` API, avoiding common NuGet package `404 Not Found` errors.

---

## 🛠️ Tech Stack

* **Framework:** ASP.NET Core 8 / 9
* **Language:** C#
* **Database:** SQLite
* **AI Model:** Google Gemini (Flash Variant)
* **Data Access:**
    * **Entity Framework Core:** For Code-First database creation and seeding.
    * **Dapper:** For executing dynamic, AI-generated SQL queries.
* **Documentation:** Swagger UI / OpenAPI

---

## 📂 Project Structure

```text
AiSqlAssistant/
├── Attributes/          # Custom attributes ([TableDescription]) for mapping schema context
├── Controllers/         # API Endpoints
├── Data/                # EF Core Context & Database Seeder
├── Models/              # Database Entities (Product, Category)
├── Services/
│   ├── AiSqlService.cs  # The "Brain" - Communicates with Google Gemini via HTTP
│   └── SchemaGenerator.cs # Reflection logic to build the database map string
├── appsettings.json     # Configuration
└── Program.cs           # Dependency Injection & Pipeline Setup
```

---

## ⚙️ Setup & Installation

### 1. Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later.
* A **Google AI Studio** API Key. (Get one for free [here](https://aistudio.google.com/)).

### 2. Clone the Repository
```bash
git clone [https://github.com/your-username/AiSqlAssistant.git](https://github.com/your-username/AiSqlAssistant.git)
cd AiSqlAssistant
```

### 3. Configure the Application
Open `appsettings.json` and paste your API Key.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  },
  "GoogleAI": {
    "ApiKey": "YOUR_GOOGLE_AI_API_KEY_HERE"
  }
}
```

### 4. Run the Project
The application includes a **Data Seeder**. When you run it for the first time, it will automatically create the `app.db` SQLite file and populate it with sample products (Electronics, Clothing, etc.).

```bash
dotnet restore
dotnet run
```

---

## 📖 Usage Guide

### Using Swagger UI
1.  Once the app is running, navigate to `https://localhost:xxxx/swagger/index.html` (the port number `xxxx` will be shown in your terminal).
2.  Locate the **POST** `/api/Query` endpoint.
3.  Click **Try it out** and enter a JSON request:

```json
{
  "question": "Show me all electronics that cost more than $500"
}
```

### Example Response
The API returns a JSON object containing the generated SQL (for transparency) and the actual data rows.

```json
{
  "generatedSql": "SELECT Name, Price FROM Products WHERE CategoryId = (SELECT Id FROM Categories WHERE Name = 'Electronics') AND Price > 500",
  "data": [
    {
      "Name": "MacBook Pro",
      "Price": 2000
    },
    {
      "Name": "iPhone 15",
      "Price": 999
    }
  ]
}
```

---

## 🔧 Troubleshooting

### "Google API Error: 404 Not Found"
This usually happens if the model name in `AiSqlService.cs` is deprecated or invalid for your region.
1.  Open `Services/AiSqlService.cs`.
2.  Check the `ModelId` constant.
    ```csharp
    // Try switching between these if one fails:
    private const string ModelId = "gemini-1.5-flash"; 
    // OR
    private const string ModelId = "gemini-2.5-flash";
    ```

### "Safety violation: Query was not SELECT"
The AI attempted to write a query that modifies data. The system correctly blocked this. Try rephrasing your question to be purely analytical (e.g., "Show me..." or "List...").

---

## ⚠️ Disclaimer

**Educational Use Only:** This project allows an AI model to generate SQL queries. While safeguards are implemented (restricting to `SELECT`), it is recommended to strictly use a **Read-Only** database user in any production environment to prevent potential prompt injection risks.

## 📄 License

[MIT](https://choosealicense.com/licenses/mit/)
