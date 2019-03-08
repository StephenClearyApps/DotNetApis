using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
    /// <summary>
    /// Objects that have a lifetime outside of the DI container. Properties retrieve the current value for the current scope.
    /// </summary>
    public static class AmbientContext
    {
        private static readonly AsyncLocal<Guid> ImplicitOperationId = new AsyncLocal<Guid>();
        private static readonly AsyncLocal<Guid> ImplicitParentOperationId = new AsyncLocal<Guid>();
        private static readonly AsyncLocal<InMemoryLoggerProvider> ImplicitInMemoryLoggerProvider = new AsyncLocal<InMemoryLoggerProvider>();
        private static readonly AsyncLocal<JsonLoggerProvider> ImplicitJsonLoggerProvider = new AsyncLocal<JsonLoggerProvider>();
        private static readonly AsyncLocal<IConfigurationRoot> ImplicitConfigurationRoot = new AsyncLocal<IConfigurationRoot>();

        public static Guid OperationId
        {
            get => ImplicitOperationId.Value;
            set => ImplicitOperationId.Value = value;
        }
        public static Guid ParentOperationId
        {
            get => ImplicitParentOperationId.Value;
            set => ImplicitParentOperationId.Value = value;
        }

        /// <summary>
        /// May return <c>null</c>.
        /// </summary>
        public static InMemoryLoggerProvider InMemoryLoggerProvider
        {
            get => ImplicitInMemoryLoggerProvider.Value;
            set => ImplicitInMemoryLoggerProvider.Value = value;
        }

        /// <summary>
        /// May return <c>null</c>.
        /// </summary>
        public static JsonLoggerProvider JsonLoggerProvider
        {
            get => ImplicitJsonLoggerProvider.Value;
            set => ImplicitJsonLoggerProvider.Value = value;
        }

        public static IConfigurationRoot ConfigurationRoot
        {
            get => ImplicitConfigurationRoot.Value;
            set => ImplicitConfigurationRoot.Value = value;
        }
    }
}
