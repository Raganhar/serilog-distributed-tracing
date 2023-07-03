using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// using OpenTelemetry;
// using OpenTelemetry.Trace;
using Serilog;

namespace dotnet31
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .Enrich.WithProperty("appName", "dotnet3")
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
            services.AddControllers();
            
            // services.AddOpenTelemetry()
            //     .WithTracing(builder => builder
            //             .AddAspNetCoreInstrumentation()
            //         // .AddJaegerExporter()
            //     )
            //     .StartWithHost();
            
            var handlerType = typeof(HttpClient).Assembly.GetType("System.Net.Http.DiagnosticsHandler");
            var listenerField = handlerType.GetField("s_diagnosticListener", BindingFlags.NonPublic | BindingFlags.Static);
            var listener = listenerField.GetValue(null) as DiagnosticListener;
            listener.Subscribe(new NullObserver(), name => false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}