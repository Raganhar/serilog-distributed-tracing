using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;
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
            
            services.AddOpenTelemetry()
                .WithTracing(builder => builder
                        .AddAspNetCoreInstrumentation()
                    // .AddJaegerExporter()
                )
                .StartWithHost();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging(opts
                => opts.EnrichDiagnosticContext = (diagnosticsContext, httpContext)=>{
                    diagnosticsContext.Set("bob","lol");
                    var request = httpContext.Request;
                    diagnosticsContext.Set("headers",request.Headers);
                    // foreach (var keyValuePair in request.Headers)
                    // {
                    //     diagnosticsContext.Set(keyValuePair.Key,keyValuePair.Value);
                    // }
                });

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}