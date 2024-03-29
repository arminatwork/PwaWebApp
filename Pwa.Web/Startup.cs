using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pwa.Infrastructure.EfCore.Extensions;
using Pwa.Infrastructure.EfCore.SeedData;
using Pwa.Query.ServiceCollection;
using WebFramework.Utilities.Sms;
using WebFramework.Utilities.Uploader;

namespace Pwa.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddHttpContextAccessor();
            services.AddScoped<IFileUploader, FileUploader>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddInitializerDatabase();
            services.AddDetection();

            services.AddExtensionsApplication(Configuration.GetConnectionString("PwaWebAppConnection"));

            services.AddQueryService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.InitializeDataBase();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute("default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
