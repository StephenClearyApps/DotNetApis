using System;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The message returned from the HTTP trigger function if it has queued a <see cref="GenerateRequestMessage"/>.
    /// </summary>
    public sealed class GenerateRequestQueuedResponseMessage : ResponseMessageBase
    {
        /// <summary>
        /// The <see cref="GenerateRequestMessage.Id"/>.
        /// </summary>
        public Guid QueuedMessageId { get; set; }
    }
}
