using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic.Formatting
{
    public sealed class StructuredFormatter
    {
        private readonly ILogger _logger;
        private readonly XmldocFormatter _xmldocFormatter;
        private readonly NameFormatter _nameFormatter;

        public StructuredFormatter(ILogger logger, XmldocFormatter xmldocFormatter, NameFormatter nameFormatter)
        {
            _logger = logger;
            _xmldocFormatter = xmldocFormatter;
            _nameFormatter = nameFormatter;
        }
    }
}
