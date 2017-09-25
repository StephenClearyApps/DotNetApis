using System;
using DotNetApis.Logic.Messages;
using DotNetApis.Storage;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The message returned from the HTTP trigger function to .
    /// </summary>
    public sealed class StatusResponseMessage : MessageBaseWithLog
    {
        public Status Status { get; set; }

        public Uri LogUri { get; set; }

        public Uri JsonUri { get; set; }
    }
}
