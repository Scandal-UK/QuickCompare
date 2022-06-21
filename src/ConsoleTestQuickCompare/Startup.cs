// <copyright file="Startup.cs" company="Dan Ware">
// Copyright (c) Dan Ware. All rights reserved.
// </copyright>

namespace ConsoleTestQuickCompare
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using QuickCompareModel;

    /// <summary>  Initialises the dependency injection container from app settings.  </summary>
    internal class Startup
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Startup"/> class.
        /// </summary>
        public Startup() =>
            this.Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

        private IConfigurationRoot Configuration { get; }

        /// <summary> Method to configure the services. </summary>
        /// <param name="services">Container of injected services.</param>
        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddSingleton(this.Configuration)
                .AddSingleton<IDifferenceBuilder, DifferenceBuilder>()
                .Configure<QuickCompareOptions>(this.Configuration.GetSection(nameof(QuickCompareOptions)));
    }
}
