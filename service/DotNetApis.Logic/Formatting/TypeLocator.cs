using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic.Assemblies;
using DotNetApis.Structure.Locations;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic.Formatting
{
    /// <summary>
    /// Finds locations (and friendly names) of types.
    /// </summary>
    public sealed class TypeLocator
    {
        private readonly ILogger<TypeLocator> _logger;
        private readonly GenerationScope.Accessor _generationScope;

        public TypeLocator(ILoggerFactory loggerFactory, GenerationScope.Accessor generationScope)
        {
            _logger = loggerFactory.CreateLogger<TypeLocator>();
            _generationScope = generationScope;
        }

        /// <summary>
        /// Returns the location and friendly name of the entity matching the specified dnaid. Note that the entity must be resolved before calling this method.
        /// </summary>
        /// <param name="dnaid">The dnaid of the entity being referenced.</param>
        public (ILocation Location, FriendlyName FriendlyName)? TryGetLocationAndFriendlyNameFromDnaId(string dnaid)
        {
            var result = _generationScope.Current.Asssemblies.TryGetLocationAndFriendlyNameFromDnaId(dnaid);
            if (result == null)
                _logger.EntityNotFound(dnaid);
            return result;
        }

        /// <summary>
        /// Returns the location of the entity matching the specified dnaid. Note that the entity must be resolved before calling this method.
        /// </summary>
        /// <param name="dnaid">The dnaid of the entity being referenced.</param>
        public ILocation TryGetLocationFromDnaId(string dnaid) => TryGetLocationAndFriendlyNameFromDnaId(dnaid)?.Location;

        /// <summary>
        /// Returns the location and friendly name of the entity matching the specified xmldocid. Note that the entity must be resolved before calling this method.
        /// </summary>
        /// <param name="xmldocid">The xmldocid of the entity being referenced.</param>
        public (ILocation Location, FriendlyName FriendlyName)? TryGetLocationAndFriendlyNameFromXmldocId(string xmldocid)
        {
            var result = _generationScope.Current.Asssemblies.TryGetLocationAndFriendlyNameFromXmldocId(xmldocid);
            if (result == null)
                _logger.XmldocEntityNotFound(xmldocid);
            return result;
        }

        /// <summary>
        /// Returns the location of the entity matching the specified xmldocid. Note that the entity must be resolved before calling this method.
        /// </summary>
        /// <param name="xmldocid">The xmldocid of the entity being referenced.</param>
        public ILocation TryGetLocationFromXmldocId(string xmldocid) => TryGetLocationAndFriendlyNameFromXmldocId(xmldocid)?.Location;
    }

    internal static partial class Logging
    {
        public static void EntityNotFound(this ILogger<TypeLocator> logger, string dnaid) =>
            Logger.Log(logger, 1, LogLevel.Warning, "Unable to find entity {dnaid}", dnaid, null);

        public static void XmldocEntityNotFound(this ILogger<TypeLocator> logger, string xmldocid) =>
            Logger.Log(logger, 2, LogLevel.Warning, "Unable to find xmldoc entity {xmldocid}", xmldocid, null);
    }
}
