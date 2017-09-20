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
    public sealed class TypeLocator
    {
        private readonly ILogger _logger;
        private readonly AssemblyCollection _assemblies;

        public TypeLocator(ILogger logger, AssemblyCollection assemblies)
        {
            _logger = logger;
            _assemblies = assemblies;
        }

        /// <summary>
        /// Returns the location and friendly name of the entity matching the specified dnaid. Note that the entity must be resolved before calling this method.
        /// </summary>
        /// <param name="dnaid">The dnaid of the entity being referenced.</param>
        public (ILocation Location, FriendlyName FriendlyName)? TryGetLocationAndFriendlyNameFromDnaId(string dnaid)
        {
            var result = _assemblies.TryGetLocationAndFriendlyNameFromDnaId(dnaid);
            if (result == null)
                _logger.LogWarning("Unable to find entity {dnaid}", dnaid);
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
            var result = _assemblies.TryGetLocationAndFriendlyNameFromXmldocId(xmldocid);
            if (result == null)
                _logger.LogWarning("Unable to find xmldoc entity {xmldocid}", xmldocid);
            return result;
        }

        /// <summary>
        /// Returns the location of the entity matching the specified xmldocid. Note that the entity must be resolved before calling this method.
        /// </summary>
        /// <param name="xmldocid">The xmldocid of the entity being referenced.</param>
        public ILocation TryGetLocationFromXmldocId(string xmldocid) => TryGetLocationAndFriendlyNameFromXmldocId(xmldocid)?.Location;
    }
}
