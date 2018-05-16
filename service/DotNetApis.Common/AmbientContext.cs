using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
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
        private static readonly AsyncLocal<string> ImplicitRequestId = new AsyncLocal<string>();
        private static readonly AsyncLocal<InMemoryLoggerProvider> ImplicitInMemoryLoggerProvider = new AsyncLocal<InMemoryLoggerProvider>();
        private static readonly AsyncLocal<JsonLoggerProvider> ImplicitJsonLoggerProvider = new AsyncLocal<JsonLoggerProvider>();

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
        public static string RequestId
        {
            get => ImplicitRequestId.Value;
            set => ImplicitRequestId.Value = value;
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

        /// <summary>
        /// Sets the values for HTTP-triggered API functions.
        /// </summary>
        public static void InitializeForHttpApi(string requestId, Guid operationId)
        {
            ImplicitOperationId.Value = operationId;
            ImplicitRequestId.Value = requestId;
        }
    }
}
