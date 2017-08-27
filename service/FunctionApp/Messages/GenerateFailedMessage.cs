using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Messages
{
    public sealed class GenerateFailedMessage : MessageBase
    {
        public GenerateFailedMessage()
            : base(includeLog: true)
        {
        }

        public string QueueMessage { get; set; }
    }
}
