using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathGame.Core.Interfaces;
using MathGame.Hubs;
using MathGame.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MathGame
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
            services.AddSignalR(x =>
            {
                x.KeepAliveInterval = TimeSpan.FromSeconds(5);
            });

            services.AddMvc();

            services.AddSingleton<SignalRMathGameClient>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IMathEquationGenerator, MathEquationGenerator>();
            services.AddTransient<IMathGameProcessor, MathGameProcessor>();
            services.AddTransient<IMathGameState, InMemoryMathGameState>();
            services.AddTransient<IRandomGenerator, PseudoRandomGenerator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSignalR(x =>
            {
                x.MapHub<MathGameHub>("/hubs/mathgame");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
