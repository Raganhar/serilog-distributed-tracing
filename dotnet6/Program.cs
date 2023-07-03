using System.Diagnostics;
using System.Net;
using System.Reflection;
using Amazon;
using Amazon.S3;
using ap.telemetry;
using DockerWebAPI;
using DockerWebAPI.Controllers;
using Flurl.Http;
using Serilog;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    .AddAutoProffTelemetryToSerilog()
    .Enrich.WithProperty("appName", "dotnet6")
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoProffTelemetry();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseSerilogRequestLogging();
app.UseRouting();
app.Run();