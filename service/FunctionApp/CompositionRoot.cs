using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace FunctionApp
{
    public static class CompositionRoot
    {
        public static async Task<Container> GetContainerForDocumentationFunctionAsync(ILogger log, TraceWriter writer, bool requestIsLocal)
        {
            var inMemoryLogger = new InMemoryLogger();
            var logger = new CompositeLogger(Enumerables.Return(inMemoryLogger, log, requestIsLocal ? new TraceWriterLogger(writer) : null));
            try
            {
                // Singletons
                var connections = await AzureConnections.ConfigureAwait(false);
                var referenceStorage = await ReferenceStorage.ConfigureAwait(false);
                var referenceAssemblies = await ReferenceAssembliesInstance.ConfigureAwait(false);

                var container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Options.DefaultLifestyle = Lifestyle.Scoped;
                container.RegisterSingleton(connections);
                container.RegisterSingleton(referenceAssemblies);
                container.RegisterSingleton<IReferenceStorage>(referenceStorage);
                container.Register(() => inMemoryLogger);
                container.Register<ILogger>(() => logger);
                container.Register<INugetRepository, NugetRepository>();
                container.Register<ILogStorage, AzureLogStorage>();
                container.Register<IStatusTable, AzureStatusTable>();
                container.Register<IPackageStorage, AzurePackageStorage>();
                container.Register<IPackageTable, AzurePackageTable>();
                container.Register<IPackageJsonTable, AzurePackageJsonTable>();
                container.Register<IPackageJsonStorage, AzurePackageJsonStorage>();
                container.Register<IReferenceXmldocTable, AzureReferenceXmldocTable>();
                container.Verify();
                return container;
            }
            catch (Exception ex)
            {
                logger.LogCritical(0, ex, "Failed to create container composition root");
                throw;
            }
        }

        private static readonly AsyncLazy<ReferenceAssemblies> ReferenceAssembliesInstance = CreateAsync(async () =>
        {
            var referenceStorage = await ReferenceStorage.ConfigureAwait(false);
            return await ReferenceAssemblies.CreateAsync(referenceStorage).ConfigureAwait(false);
        });

        private static readonly AsyncLazy<AzureReferenceStorage> ReferenceStorage = CreateAsync(async () =>
        {
            var connections = await AzureConnections.ConfigureAwait(false);
            var result = new AzureReferenceStorage(connections);
            return result;
        });

        private static readonly AsyncLazy<AzureConnections> AzureConnections = CreateAsync(async () =>
        {
            var result = new AzureConnections();
            await result.InitializeAsync().ConfigureAwait(false);
            await Task.WhenAll(AzurePackageStorage.InitializeAsync(result),
                    AzurePackageTable.InitializeAsync(result),
                    AzurePackageJsonTable.InitializeAsync(result),
                    AzurePackageJsonStorage.InitializeAsync(result),
                    AzureReferenceStorage.InitializeAsync(result),
                    AzureReferenceXmldocTable.InitializeAsync(result))
                .ConfigureAwait(false);
            return result;
        });

        private static AsyncLazy<T> CreateAsync<T>(Func<Task<T>> factory) => new AsyncLazy<T>(factory, AsyncLazyFlags.ExecuteOnCallingThread | AsyncLazyFlags.RetryOnFailure);
    }
}
