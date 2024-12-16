using ISSTechLogistics.Data;
using ISSTechLogistics.Models.Services;
using ISSTechLogistics.Repository.Orders;
using ISSTechLogistics.Repository.OrdersDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read configuration from appsettings.json
    .WriteTo.Console() // Log to console
    .CreateLogger();

builder.Logging.ClearProviders(); // Remove default logging providers
builder.Logging.AddSerilog(); // Add Serilog

// Register Serilog's ILogger in the DI container
builder.Services.AddSingleton(Log.Logger);

builder.Services.AddControllers();

// SQL Server database connection settings
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the OrdersRepository
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<IOrdersDetailsStatisticsRepository, OrdersDetailsStatisticsRepository>();

builder.Services.Configure<LogisticsTechSettings>(builder.Configuration.GetSection("LogisticsTechSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting the application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}