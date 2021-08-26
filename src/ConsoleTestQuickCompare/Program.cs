namespace TestQuickCompare
{
    using System;
    using System.Diagnostics;
    using Microsoft.Extensions.DependencyInjection;
    using QuickCompareModel;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var provider = GetServiceProvider();
                var builder = provider.GetService<IDifferenceBuilder>();

                builder.ComparisonStatusChanged += OnComparisonStatusChanged;
                builder.BuildDifferences();

                Console.WriteLine("\r\n--------------------------------");

                var report = builder.Differences.ToString();
                Console.Write(report);
                Trace.Write(report);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                Trace.Write(ex.ToString());
            }
            finally
            {
                Console.ReadKey();
            }
        }

        /// <summary> Gets the <see cref="IServiceProvider"/> DI container configured in <see cref="Startup"/>. </summary>
        /// <returns> An instance of <see cref="IServiceProvider"/>. </returns>
        private static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            new Startup().ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        private static void OnComparisonStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Console.Write($"\r{e.StatusMessage,-70}");
            Trace.WriteLine(e.StatusMessage);
        }
    }
}
