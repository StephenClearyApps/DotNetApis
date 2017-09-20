using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Returns the location of the DnaId within the context. Note that the entity must be resolved before calling this method.
        /// </summary>
        /// <param name="dnaid">The DnaId of the entity being referenced.</param>
        public ILocation TryGetLocationFromDnaId(string dnaid)
        {
            var result = _assemblies.TryGetLocationAndFriendlyNameFromDnaId(dnaid)?.Location;
            if (result == null)
                _logger.LogWarning("Unable to find location of entity {dnaid}", dnaid);
            return result;
        }
    }
}
