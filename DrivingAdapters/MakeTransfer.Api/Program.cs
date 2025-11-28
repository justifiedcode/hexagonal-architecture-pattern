using MakeTransfer.Core.Application.Ports.Incoming;
using MakeTransfer.Core.Application.UseCases;
using MakeTransfer.Adapters.Database;
using MakeTransfer.Adapters.Payment;
using MakeTransfer.Adapters.Notification;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Basic web API setup
builder.Services.AddControllers();

// API documentation with Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MakeTransfer API",
        Version = "v1",
        Description = "Hexagonal Architecture Demo - Money Transfer API with Swappable Secondary Adapters",
        Contact = new OpenApiContact
        {
            Name = "MakeTransfer API",
            Email = "support@maketransfer.com"
        }
    });

    // Include XML docs if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Register our core business logic
builder.Services.AddScoped<IBankingOperationsInputPort, BankingOperationsService>();
builder.Services.AddScoped<IBankingInquiryInputPort, BankingInquiryService>();

// Set up our adapter implementations
Console.WriteLine("Setting up adapters...");

// Database adapter
builder.Services.AddInMemoryDatabaseAdapter();
Console.WriteLine("Database: In-memory storage ready");

// Payment gateway
builder.Services.AddPaymentGatewayAdapter();
Console.WriteLine("   Payment Gateway: Simulation adapter loaded");

// Notifications
builder.Services.AddConsoleNotificationAdapter();
Console.WriteLine("   Notifications: Console adapter loaded");

Console.WriteLine("All adapters configured successfully!");

// CORS for frontend apps
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Logging setup
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    if (builder.Environment.IsDevelopment())
    {
        logging.SetMinimumLevel(LogLevel.Debug);
    }
});

var app = builder.Build();

// Development goodies
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MakeTransfer API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "MakeTransfer API - Hexagonal Architecture Demo";
        options.DefaultModelsExpandDepth(-1);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// API info endpoint
app.MapGet("/", () => new
{
    Message = "Welcome to MakeTransfer API - Hexagonal Architecture Demo",
    Architecture = new
    {
        Pattern = "Hexagonal (Ports & Adapters)",
        PrimaryAdapters = new[] { "REST API Controllers" },
        SecondaryAdapters = new[]
        {
        "Database Adapter (In-Memory)",
        "Payment Gateway Adapter (Simulation)",
        "Notification Adapter (Console)"
        }
    },
    Documentation = new
    {
        SwaggerUI = "/swagger",
        OpenAPISpec = "/swagger/v1/swagger.json"
    },
    Endpoints = new
    {
        Transfers = "/api/transfers",
        Accounts = "/api/accounts/{accountId}",
        Balance = "/api/accounts/{accountId}/balance",
        Health = "/api/transfers/health"
    },
    Configuration = new
    {
        Environment = app.Environment.EnvironmentName
    },
    Version = "2.0.0",
    Timestamp = DateTime.UtcNow,
    SampleAccounts = new[]
    {
        new { AccountId = "ACC001", Owner = "John Doe", Currency = "USD", Balance = "5000", DailyLimit = "1000" },
        new { AccountId = "ACC002", Owner = "Jane Smith", Currency = "USD", Balance = "2500", DailyLimit = "500" },
        new { AccountId = "ACC003", Owner = "Bob Johnson", Currency = "EUR", Balance = "3000", DailyLimit = "750" },
        new { AccountId = "ACC004", Owner = "Alice Brown", Currency = "USD", Balance = "10000", DailyLimit = "2000" },
        new { AccountId = "ACC005", Owner = "Charlie Wilson", Currency = "GBP", Balance = "4500", DailyLimit = "800" }
    }
});

Console.WriteLine($"MakeTransfer API starting on {app.Environment.EnvironmentName} environment");
Console.WriteLine("Adapter configuration:");
Console.WriteLine($"   Database: InMemoryDatabaseAdapter");
Console.WriteLine($"   Payment Gateway: PaymentGatewayAdapter");
Console.WriteLine($"   Notifications: Console");

app.Run();