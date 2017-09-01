using System;
using Logic.Messages;
using Storage;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The message returned from the HTTP trigger function to .
    /// </summary>
    public sealed class StatusResponseMessage : MessageBase
    {
        public StatusResponseMessage()
            : base(includeLog: true)
        {
        }

        public Status Status { get; set; }

        public Uri LogUri { get; set; }
    }
}
