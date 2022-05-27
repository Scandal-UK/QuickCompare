// <copyright file="Startup.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace ConsoleTestQuickCompare
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using QuickCompareModel;

    /// <summary>
    /// Initialises the DI container from app settings.
    /// </summary>
    internal class Startup
    {
        private IConfigurationRoot Configuration { get; }

        public Startup() =>
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddSingleton(Configuration)
                .AddSingleton<IDifferenceBuilder, DifferenceBuilder>()
                .Configure<QuickCompareOptions>(Configuration.GetSection(nameof(QuickCompareOptions)));
    }
}
