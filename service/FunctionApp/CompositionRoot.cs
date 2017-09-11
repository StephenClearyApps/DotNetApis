using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using DotNetApis.Storage;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace FunctionApp
{
    public static class CompositionRoot
    {
        public static async Task<Container> GetContainerForDocumentationFunctionAsync()
        {
            var connections = await AzureConnections.ConfigureAwait(false);

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.DefaultLifestyle = Lifestyle.Scoped;
            container.RegisterSingleton(connections);
            container.Register<IReferenceStorage, AzureReferenceStorage>();
            container.Register<ILogger, AmbientCompositeLogger>();
            container.Register<INugetRepository, NugetRepository>();
            container.Register<IPackageStorage, AzurePackageStorage>();
            container.Register<IPackageTable, AzurePackageTable>();
            container.Register<IPackageJsonTable, AzurePackageJsonTable>();
            container.Register<IPackageJsonStorage, AzurePackageJsonStorage>();
            container.Register<ILogStorage, AzureLogStorage>();
            container.Register<IStatusTable, AzureStatusTable>();
            container.Register<IReferenceXmldocTable, AzureReferenceXmldocTable>();
            container.Verify();
            return container;
        }

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
