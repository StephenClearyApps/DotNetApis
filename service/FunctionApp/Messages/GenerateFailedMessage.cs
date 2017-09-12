using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Common;
using DotNetApis.Logic.Messages;

namespace FunctionApp.Messages
{
    public sealed class GenerateFailedMessage : MessageBase
    {
        public GenerateFailedMessage(InMemoryLogger inMemoryLogger)
            : base(inMemoryLogger)
        {
        }

        public string QueueMessage { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }
    }
}
