using AutoMapper;
using Domain.Entities.Identity;
using Domain.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Persistence.ContextModel;
using Utility.Factory;


namespace WebPublicMVC;

public class Startup
{
    public IConfiguration Configuration { get; }
    public DependencyResolver DependencyResolver { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        DependencyResolver = new DependencyResolver();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("Persistence")));

        //services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        services.AddControllersWithViews();
        services.AddRazorPages();

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;

            // Lockout settings
            //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        });

        services.Configure<PasswordHasherOptions>(options =>
            options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3
        );

        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AppAutoMapperProfile());
        });
        services.AddSingleton(mappingConfig.CreateMapper());

        DependencyResolver.SetDependencyConfiguration(services);

        services.AddAutoMapper(typeof(Startup));

        services.Configure<AppSettings>(Configuration);

        services.AddMemoryCache();

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
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();

        //For Ajax--
        app.UseStaticFiles();
        app.UseDefaultFiles();
        var staticFileOptions = new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), Domain.Utility.Utility.UploadingFolderPath)),
            RequestPath = new PathString(Configuration[Domain.Utility.Utility.UploadingFolderPath])
        };
        app.UseStaticFiles(staticFileOptions);
        Domain.Utility.Utility.ServerPath = env.WebRootPath;
        Domain.Utility.Utility.ProjectPhysicalPath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), Domain.Utility.Utility.UploadingFolderPath)).Root;

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            //endpoints.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapRazorPages();
        });
    }
}
