using AiSqlAssistant.Api.Data;
using AiSqlAssistant.Api.Models;
using AiSqlAssistant.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// 1. Setup Database
builder.Services.AddDbContext<AppDbContext>(opts => 
    opts.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Setup AI (Semantic Kernel)
var googleApiKey = builder.Configuration["GoogleAI:ApiKey"];
var modelId = "gemini-flash-latest";
builder.Services.AddKernel()
    .AddGoogleAIGeminiChatCompletion(
    modelId: modelId,
    apiKey: googleApiKey!);

// 3. Register Custom Services
builder.Services.AddScoped<SchemaGenerator>();
builder.Services.AddScoped<AiSqlService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Auto-Seed Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    
    if (!context.Products.Any())
    {
        var elec = new Category { Name = "Electronics" };
        var cloth = new Category { Name = "Clothing" };
        context.Categories.AddRange(elec, cloth);
        
        context.Products.AddRange(
            new Product { Name = "MacBook Pro", Price = 2000, Stock = 10, Category = elec },
            new Product { Name = "iPhone 15", Price = 999, Stock = 0, Category = elec },
            new Product { Name = "Winter Coat", Price = 150, Stock = 5, Category = cloth }
        );
        context.SaveChanges();
    }
}

// 5. Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();