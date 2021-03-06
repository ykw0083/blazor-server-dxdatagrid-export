using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DxDataGridExportingWithReports.Data;
using DxDataGridExportingWithReports.Helpers;
using DxDataGridExportingWithReports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace DxDataGridExportingWithReports
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        const string corsPolicy = "CorsPolicy";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {


            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton(Configuration);
            services.AddTransient<ExportMiddleware>();
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy, builder =>
                 builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .Build());

            });

            services.AddDbContext<SpDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddScoped<ISpDBContext, SpDBContext>();

            //Action<MvcNewtonsoftJsonOptions> JsonOptions = options =>
            //{
            //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //};
            //services.AddMvc()
            //    .AddNewtonsoftJson(
            //        options => {
            //                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //            }

            //    );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware(typeof(ExportMiddleware));
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(corsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers(); // enable controller to be called
            });
        }
    }
}
