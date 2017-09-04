using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic.Formatting
{
    public sealed class XmldocFormatter
    {
        private readonly ILogger _logger;

        public XmldocFormatter(ILogger logger)
        {
            _logger = logger;
        }

        
    }
}
