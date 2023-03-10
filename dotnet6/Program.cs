using System.Diagnostics;
using System.Net;
using System.Reflection;
using Amazon;
using Amazon.S3;
using DockerWebAPI;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.Aws.S3;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    .Enrich.WithProperty("appName", "dotnet6")
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
builder.Host.UseSerilog();

builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var handlerType = typeof(HttpClient).Assembly.GetType("System.Net.Http.DiagnosticsHandler");
var listenerField = handlerType.GetField("s_diagnosticListener", BindingFlags.NonPublic | BindingFlags.Static);
var listener = listenerField.GetValue(null) as DiagnosticListener;
listener.Subscribe(new NullObserver(), name => false);

// builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetry()
    .WithTracing(builder => builder
            .AddAspNetCoreInstrumentation()
        // .AddJaegerExporter()
    )
    .StartWithHost();
// builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseSerilogRequestLogging();
app.UseRouting();
app.Run();