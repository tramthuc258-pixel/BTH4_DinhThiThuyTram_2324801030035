using Newtonsoft.Json.Converters;
using Npgsql;
using PricingService.Commands;
using PricingService.Controllers;
using PricingService.Data;
using Steeltoe.Discovery.Client;

namespace PricingService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDiscoveryClient();
        services.AddControllers(options => options.Filters.Add<PricingExceptionFilter>())
            .AddApplicationPart(typeof(PricingController).Assembly)
            .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining<CalculatePriceHandler>());
        services.AddSingleton(_ => NpgsqlDataSource.Create(
            Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection for PricingService.")));
        services.AddSingleton<PostgresDataStore>();
        services.AddSingleton<IDataStore>(provider => provider.GetRequiredService<PostgresDataStore>());
        services.AddHostedService<TariffDatabaseInitializer>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
