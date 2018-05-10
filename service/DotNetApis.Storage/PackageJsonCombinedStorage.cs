﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Nuget;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetApis.Storage
{
    public sealed class PackageJsonCombinedStorage
    {
        private readonly ILogger<PackageJsonCombinedStorage> _logger;
        private readonly IPackageJsonTable _table;
        private readonly IPackageJsonStorage _storage;

        public PackageJsonCombinedStorage(ILoggerFactory loggerFactory, IPackageJsonTable table, IPackageJsonStorage storage)
        {
            _logger = loggerFactory.CreateLogger<PackageJsonCombinedStorage>();
            _table = table;
            _storage = storage;
        }

		/// <summary>
		/// Writes a complete JSON string to storage.
		/// </summary>
		/// <param name="idver">The id and version of the package.</param>
		/// <param name="target">The target for the package.</param>
		/// <param name="docJson">The JSON documentation for the specified package id, version, and target.</param>
		/// <param name="log">The log writer.</param>
        public async Task WriteSuccessAsync(NugetPackageIdVersion idver, PlatformTarget target, string docJson, IJsonBlobWriter log)
        {
            // Save the JSON documentation to blob storage.
            _logger.SavingJson(idver, target);
            var stopwatch = Stopwatch.StartNew();
            var jsonUri = await _storage.WriteJsonAsync(idver, target, docJson).ConfigureAwait(false);
            _logger.SavedJson(idver, target, jsonUri, stopwatch.Elapsed);

            // Save the processing log to blob storage.
            var logUri = await SaveLogAsync(log).ConfigureAwait(false);

            // Mark the processing as complete.
            await _table.SetRecordAsync(idver, target, Status.Succeeded, logUri, jsonUri).ConfigureAwait(false);
        }

		/// <summary>
		/// Writes a failure notification to storage.
		/// </summary>
		/// <param name="idver">The id and version of the package.</param>
		/// <param name="target">The target for the package.</param>
		/// <param name="log">The log writer.</param>
		public async Task WriteFailureAsync(NugetPackageIdVersion idver, PlatformTarget target, IJsonBlobWriter log)
        {
            // Save the processing log to blob storage.
            var logUri = await SaveLogAsync(log).ConfigureAwait(false);

            // Mark the processing as failed.
            await _table.SetRecordAsync(idver, target, Status.Failed, logUri, null).ConfigureAwait(false);
        }

        private static async Task<Uri> SaveLogAsync(IJsonBlobWriter log)
        {
	        await (AmbientContext.JsonLoggerProvider?.StopAsync() ?? Task.CompletedTask).ConfigureAwait(false);
	        await log.CommitAsync().ConfigureAwait(false);
	        return log.Uri;
        }
    }

	internal static partial class Logging
	{
		public static void SavingJson(this ILogger<PackageJsonCombinedStorage> logger, NugetPackageIdVersion idver, PlatformTarget target) =>
			Logger.Log(logger, 1, LogLevel.Debug, "Saving json for {idver} target {target}", idver, target, null);

		public static void SavedJson(this ILogger<PackageJsonCombinedStorage> logger, NugetPackageIdVersion idver, PlatformTarget target, Uri uri, TimeSpan elapsed) =>
			Logger.Log(logger, 2, LogLevel.Debug, "Saved json for {idver} target {target} at {uri} in {elapsed}", idver, target, uri, elapsed, null);
	}
}
