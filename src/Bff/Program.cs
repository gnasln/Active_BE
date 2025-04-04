using System.Configuration;
using Bff.Infrastructure.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using PerfSvc.Application;
using TodoSvc.Application;
using TodoSvc.Infrastructure;
using Serilog;
using Serilog.Formatting.Json;
using Elastic.Apm.NetCoreAll;
using PerfSvc.Infrastructure;

// log
var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Add services to the container

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();
builder.Services.AddControllers();

#region Add module / microservice
// TodoModule:
builder.Services.AddTodoServices();
builder.Services.AddTodoInfrastructureServices(builder.Configuration);
builder.Services.AddPerfServices();
builder.Services.AddPerfInfrastructureServices(builder.Configuration);
#endregion

// log
builder.Host.UseSerilog((ctx, config) =>
{
    config.WriteTo.Console().MinimumLevel.Information();
    config.WriteTo.File(
      path: AppDomain.CurrentDomain.BaseDirectory + "/logs/log-.txt",
      rollingInterval: RollingInterval.Day,
      rollOnFileSizeLimit: true,
      formatter: new JsonFormatter()).MinimumLevel.Information();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT token into authorization header using the Bearer scheme. Example: Bearer {token}",
        Name = "Authorization",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            { new OpenApiSecurityScheme
                { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }
            , new List<string>() }
        });

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Active API root", Version = "v1" });
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.Services.AddFluentEmail(builder.Configuration);

var app = builder.Build();

// use log
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});
app.UseCertificateForwarding();
app.UseHsts();
if (app.Environment.IsDevelopment() || app.Configuration["EnableDumpEnv"] == "1")
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.InitializeDatabaseAsync();
    // await app.Services.InitializeTodoDatabaseAsync();
    //await app.Services.InitializePerfDatabaseAsync();
}


app.UseHealthChecks("/healthz");

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
app.MapEndpoints();

app.Run();