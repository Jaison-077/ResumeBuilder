using Microsoft.OpenApi.Models;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Application.Services;
using ResumeBuilder.Infrastructure.DocumentGeneration;
using ResumeBuilder.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// ============ SERVICE REGISTRATION ============

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep original casing
    });

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            builder.Configuration["Cors:AllowedOrigins"]?.Split(';') ?? new[] { "http://localhost:4200" }
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Resume Generator & ATS Optimizer API",
        Version = "v1",
        Description = "API for generating, refactoring, and optimizing resumes using Azure OpenAI",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Url = new Uri("https://example.com")
        }
    });
});

// Add Application Services
builder.Services.AddScoped<IResumeContentService, ResumeContentService>();
builder.Services.AddScoped<IATSService, ATSService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IFileExtractionService, FileExtractionService>();

// Add Infrastructure Services
// Azure OpenAI Service
var azureOpenAiEndpoint = builder.Configuration["AzureOpenAI:Endpoint"] 
    ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:Endpoint");
var azureOpenAiKey = builder.Configuration["AzureOpenAI:Key"] 
    ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:Key");
var azureOpenAiDeployment = builder.Configuration["AzureOpenAI:DeploymentName"] 
    ?? throw new InvalidOperationException("Missing configuration: AzureOpenAI:DeploymentName");

builder.Services.AddScoped<IAIService>(sp =>
    new AzureOpenAiService(
        sp.GetRequiredService<ILogger<AzureOpenAiService>>(),
        azureOpenAiEndpoint,
        azureOpenAiKey,
        azureOpenAiDeployment
    )
);

// Azure Blob Storage Service
var blobConnectionString = builder.Configuration["AzureBlob:ConnectionString"] 
    ?? throw new InvalidOperationException("Missing configuration: AzureBlob:ConnectionString");
var blobContainerName = builder.Configuration["AzureBlob:ContainerName"] ?? "resumes";

builder.Services.AddScoped<IStorageService>(sp =>
    new AzureBlobStorageService(
        sp.GetRequiredService<ILogger<AzureBlobStorageService>>(),
        blobConnectionString,
        blobContainerName
    )
);

// Add Logging
builder.Services.AddLogging(options =>
{
    options.AddConsole();
    options.AddDebug();
    if (!builder.Environment.IsDevelopment())
    {
        // TODO: Add Application Insights logging in production
        // options.AddApplicationInsights();
    }
});

// ============ BUILD APP & CONFIGURE MIDDLEWARE ============

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Resume Builder API v1");
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}

// HTTPS redirection should be after environment check
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable CORS
app.UseCors("AllowFrontend");

// Global error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow })
    .WithName("Health Check")
    .WithOpenApi();

app.Run();

/// <summary>
/// Global error handling middleware.
/// Catches unhandled exceptions and returns consistent error responses.
/// </summary>
internal class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            message = "An internal server error occurred",
            error = exception.Message,
            timestamp = DateTime.UtcNow
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
