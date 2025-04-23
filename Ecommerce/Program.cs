using Ecommerce.Models;
using Ecommerce.Services;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Stripe;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- Konfiguration --------------------------------------------------

            // MongoDB
            builder.Services.Configure<MongoSettings>(
                builder.Configuration.GetSection("MongoSettings"));

            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
                return new MongoClient(mongoSettings.ConnectionString);
            });

            // Stripe
            builder.Services.Configure<StripeSettings>(
                builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            // Identity + roller (MongoDB)
            builder.Services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
            }, mongoOptions =>
            {
                mongoOptions.ConnectionString = builder.Configuration["MongoSettings:ConnectionString"];
                mongoOptions.UsersCollection = "Users";
                mongoOptions.RolesCollection = "Roles";
            });

            // Authorization-policy
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
            });

            // Applikationstjänster
            builder.Services.AddSingleton<IProductService, Services.ProductService>();
            builder.Services.AddSingleton<IReviewService, Services.ReviewService>();
            builder.Services.AddSingleton<IOrderService, OrderService>();

            // Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // --- Seeda roller & admin-användare ---------------------------------

            using (var scope = app.Services.CreateScope())
            {
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Skapa roller om de inte finns
                if (!await roleMgr.RoleExistsAsync("admin"))
                    await roleMgr.CreateAsync(new ApplicationRole { Name = "admin" });

                if (!await roleMgr.RoleExistsAsync("user"))
                    await roleMgr.CreateAsync(new ApplicationRole { Name = "user" });

                // Hämta admin-email från appsettings
                var adminEmail = builder.Configuration["Admin:Email"];

                if (!string.IsNullOrWhiteSpace(adminEmail))
                {
                    var admin = await userMgr.FindByEmailAsync(adminEmail);
                    if (admin != null)
                    {
                        var roles = await userMgr.GetRolesAsync(admin);
                        if (!roles.Contains("admin"))
                        {
                            await userMgr.AddToRoleAsync(admin, "admin");
                        }
                    }
                }
            }

            // --- Middleware -----------------------------------------------------

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            // --- Routing --------------------------------------------------------

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
