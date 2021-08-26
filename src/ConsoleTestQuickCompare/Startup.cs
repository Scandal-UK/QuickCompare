namespace TestQuickCompare
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using QuickCompareModel;

    /// <summary>
    /// Initialises the DI container from app settings.
    /// </summary>
    internal class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddSingleton(Configuration)
                .AddSingleton<IDifferenceBuilder, DifferenceBuilder>()
                .Configure<QuickCompareOptions>(Configuration.GetSection(nameof(QuickCompareOptions)));
    }
}
