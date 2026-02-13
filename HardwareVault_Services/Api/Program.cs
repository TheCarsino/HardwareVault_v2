using HardwareVault_Services.Application.Interfaces;
using HardwareVault_Services.Application.Services;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Infrastructure.Parsers;
using HardwareVault_Services.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;


var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ConfiguredCors", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("SQLServerConnection")
    ?? throw new InvalidOperationException(
        "Connection string not found.");

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            // Enable retry on failure (for transient errors)
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);

            // Command timeout
            sqlOptions.CommandTimeout(60);
        });

    // Enable detailed errors in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Register repositories and Unit of Work
// All repos share the same UoW instance which shares the same DbContext.
// This means all changes within one request are in one transaction.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<ICpuRepository, CpuRepository>();
builder.Services.AddScoped<IGpuRepository, GpuRepository>();
builder.Services.AddScoped<IPowerSupplyRepository, PowerSupplyRepository>();
builder.Services.AddScoped<IImportJobRepository, ImportJobRepository>();

// Register Excel parser
builder.Services.AddScoped<IExcelDeviceParser, ExcelDeviceParser>();

// Register services
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
builder.Services.AddScoped<IImportService, ImportService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//User Credentials to Read Excel Files
ExcelPackage.License.SetNonCommercialPersonal("HardwareVault Dev");

var app = builder.Build();

app.UseCors("ConfiguredCors");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
