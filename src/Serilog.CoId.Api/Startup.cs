using CorrelationId;
using CorrelationId.DependencyInjection;
using CorrelationId.HttpClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Serilog.CoId.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddTransient<NoOpDelegatingHandler>();

            services.AddHttpClient("CalledClient", opts =>
            {
                opts.BaseAddress = new Uri("https://localhost:5003");
            })
            .AddCorrelationIdForwarding();
            //.AddHttpMessageHandler<NoOpDelegatingHandler>();


            services.AddDefaultCorrelationId(options =>
            {
                options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
                options.AddToLoggingScope = true;
                options.EnforceHeader = true;
                options.IncludeInResponse = true;
                options.RequestHeader = "X-Correlation-Id";
                options.ResponseHeader = "X-Correlation-Id";
                options.UpdateTraceIdentifier = false;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseCorrelationId();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Running...");
                });
            });
        }
    }
}
