using Serilog;
using Serilog.Events;

namespace WebPublicMVC;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsetting.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configBuilder = builder.Build();

        string date = DateTime.Now.ToString("dd-MMM-yyyy");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(@"logs/log-" + date + "file.txt")
            .CreateLogger();

        try
        {
            Log.Information("Hotel Management is starting up");
            CreateHostBuilder(args).Build().Run();
        }

        catch (Exception ex)
        {
            Log.Fatal(ex, "Hotel Management start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.UseWebRoot("wwwroot");
                webBuilder.UseStartup<Startup>();
                webBuilder.UseIIS();
            });
}

