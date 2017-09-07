using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Logic
{
    public sealed class ProcessReferenceXmldocHandler
    {
        private readonly ILogger _logger;

        public ProcessReferenceXmldocHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync()
        {
            await Task.Yield();
            throw new NotImplementedException();
        }
    }
}
