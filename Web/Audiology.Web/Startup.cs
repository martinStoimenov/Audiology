namespace Audiology.Web
{
    using System;
    using System.Reflection;

    using Audiology.Data;
    using Audiology.Data.Common;
    using Audiology.Data.Common.Repositories;
    using Audiology.Data.Models;
    using Audiology.Data.Repositories;
    using Audiology.Data.Seeding;
    using Audiology.Services.Data.Administration;
    using Audiology.Services.Data.Albums;
    using Audiology.Services.Data.Comments;
    using Audiology.Services.Data.Favourites;
    using Audiology.Services.Data.Playlists;
    using Audiology.Services.Data.Profile;
    using Audiology.Services.Data.Songs;
    using Audiology.Services.Mapping;
    using Audiology.Services.Messaging;
    using Audiology.Web.Hubs;
    using Audiology.Web.ViewModels;
    using CloudinaryDotNet;
    using Hangfire;
    using Hangfire.Dashboard;
    using Hangfire.SqlServer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); // CSRF
            });
            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder() // Add default authorization for all controllers
                    .RequireAuthenticatedUser()
                    .Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddSingleton(this.configuration);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(this.configuration["SendGrid:AppKey"]));
            services.AddTransient<ISongsServcie, SongsService>();
            services.AddTransient<IAlbumsService, AlbumsService>();
            services.AddTransient<IFavouritesService, FavouritesService>();
            services.AddTransient<IPlaylistsService, PlaylistsService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<ICommentsService, CommentsService>();
            services.AddTransient<IAdminService, AdminService>();

            // Cloudinary
            Account account = new Account(
                this.configuration["Cloudinary:AppName"],
                this.configuration["Cloudinary:AppKey"],
                this.configuration["Cloudinary:AppSecret"]);

            Cloudinary cloudinary = new Cloudinary(account);

            services.AddSingleton(cloudinary);

            // SignalR
            services.AddSignalR(options =>
                options.EnableDetailedErrors = true)
                .AddMessagePackProtocol();

            // Hangfire
            services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(this.configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true,
    }));

            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }

                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("Home/Error");
                app.UseHsts();
            }

            app.UseHangfireDashboard("/hangfire", new DashboardOptions { IsReadOnlyFunc = (DashboardContext context) => true });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapHub<ChatHub>("/chat");
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapRazorPages();
                    });

            recurringJobManager.AddOrUpdate("Gather-song-lyrics", () => serviceProvider.GetService<ISongsServcie>().BackgroundLyricsGathering(), Cron.Weekly(DayOfWeek.Friday));
        }
    }
}
