using System.Diagnostics;
using Dapper;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Serilog;

namespace DockerWebAPI.Controllers;

[ApiController]
[Route("")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<string> Get()
    {
        var a = Activity.Current.Context;
        Log.Information($"About to call external service contxt: trace:{a.TraceId} spanid {a.SpanId}");
        var stringAsync =await "https://localhost:44386/weatherforecast".GetStringAsync();
        Console.WriteLine(stringAsync);
        return "hello b";
    }
}