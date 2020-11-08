using System;
using aspCart.Core.Interface.Services.Catalog;
using aspCart.Core.Interface.Services.Messages;
using aspCart.Core.Interface.Services.Sale;
using aspCart.Core.Interface.Services.Statistics;
using aspCart.Core.Interface.Services.User;
using aspCart.Infrastructure;
using aspCart.Infrastructure.EFModels;
using aspCart.Infrastructure.EFRepository;
using aspCart.Infrastructure.Services.Catalog;
using aspCart.Infrastructure.Services.Messages;
using aspCart.Infrastructure.Services.Sale;
using aspCart.Infrastructure.Services.Statistics;
using aspCart.Infrastructure.Services.User;
using aspCart.Web.Areas.Admin.Helpers;
using aspCart.Web.Helpers;
using aspCart.Web.Middleware;
using aspCart.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace aspCart.Web
{
    public class Startup
    {
        [Obsolete]
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets("aspnet-aspCart.Web-b7b6c0c8-2794-41a1-ad6c-528772b97f8a");
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            HostingEnvironment = env;

            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfileConfiguration());
            });
        }

        public IConfigurationRoot Configuration { get; }

        public MapperConfiguration MapperConfiguration { get; set; }

        [Obsolete]
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment;

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("tr-TR");
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                     options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                     b => b.MigrationsAssembly("aspCart.Web")));

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<AdminAccount>(
                Configuration.GetSection("AdminAccount"));

            services.Configure<UserAccount>(
                Configuration.GetSection("UserAccount"));

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();

            // Add application services.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IBillingAddressService, BillingAddressService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IImageManagerService, ImageManagerService>();
            services.AddTransient<IManufacturerService, ManufacturerService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IReviewService, ReviewService>();
            services.AddTransient<ISpecificationService, SpecificationService>();

            services.AddTransient<IOrderCountService, OrderCountService>();
            services.AddTransient<IVisitorCountService, VisitorCountService>();

            services.AddTransient<IContactUsService, ContactUsService>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.Name = "aspCart";
            });

            services.AddSingleton(sp => MapperConfiguration.CreateMapper());

            services.AddScoped<ViewHelper>();
            services.AddScoped<DataHelper>();
            services.AddSingleton(HostingEnvironment.ContentRootFileProvider);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                var options = new RewriteOptions().AddRedirectToHttpsPermanent();
                app.UseRewriter(options);
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseVisitorCounter();
            app.UseRequestLocalization();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Dashboard", action = "Index" });

                routes.MapControllerRoute(
                    name: "productInfo",
                    pattern: "Product/{seo}",
                    defaults: new { controller = "Home", action = "ProductInfo" });

                routes.MapControllerRoute(
                    name: "category",
                    pattern: "Category/{category}",
                    defaults: new { controller = "Home", action = "ProductCategory" });

                routes.MapControllerRoute(
                    name: "manufacturer",
                    pattern: "Manufacturer/{manufacturer}",
                    defaults: new { controller = "Home", action = "ProductManufacturer" });

                routes.MapControllerRoute(
                    name: "productSearch",
                    pattern: "search/{name?}",
                    defaults: new { controller = "Home", action = "ProductSearch" });

                routes.MapControllerRoute(
                    name: "create review",
                    pattern: "CreateReview/{id}",
                    defaults: new { controller = "Home", action = "CreateReview" });

                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //SampleDataProvider.ApplyMigration(app.ApplicationServices);

            //SampleDataProvider.Seed(app.ApplicationServices, Configuration);
        }
    }
}