using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Messages;

namespace FunctionApp.Messages
{
    public sealed class GenerateFailedMessage : MessageBase
    {
        public GenerateFailedMessage()
            : base(includeLog: true)
        {
        }

        public string QueueMessage { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }
    }
}
