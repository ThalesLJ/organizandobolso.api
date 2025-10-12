using Microsoft.OpenApi.Models;
using OrganizandoBolso.API.Controllers;
using OrganizandoBolso.API.Filters;
using OrganizandoBolso.API.Middleware;
using OrganizandoBolso.Application.Services;
using OrganizandoBolso.Domain.Configuration;
using OrganizandoBolso.Domain.Interfaces;
using OrganizandoBolso.Repository.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<AuditFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrganizandoBolso API",
        Version = "v1",
        Description = "API for personal budgets and expenses management",
        Contact = new OpenApiContact
        {
            Name = "OrganizandoBolso",
            Email = "thaleslimadejesus@gmail.com"
        }
    });

    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = builder.Configuration["MONGODB_URI"] ?? string.Empty;
    options.DatabaseName = "OrganizandoBolso";
});
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

builder.Services.AddMemoryCache();



builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddEventSourceLogger();
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();

builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<GlobalExceptionFilter>();
builder.Services.AddScoped<AuditFilter>();

builder.Services.AddHostedService<OrganizandoBolso.Repository.HostedServices.MongoConnectionHostedService>();

var app = builder.Build();

app.UseSwagger();
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
    if (string.Equals(path, "/api/docs", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(path, "/api/docs/", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/api/docs/index.html";
    }
    await next();
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrganizandoBolso API v1");
    c.RoutePrefix = "api/docs";
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseResponseCompression();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TimezoneMiddleware>();
app.UseMiddleware<JwtSessionMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
