﻿using System;

namespace FunctionApp.Messages
{
    /// <summary>
    /// The message sent from the HTTP trigger function to the queue trigger function.
    /// </summary>
    public sealed class GenerateRequestMessage : MessageBase
    {
        public GenerateRequestMessage()
            : base(includeLog: false)
        {
        }

        /// <summary>
        /// The schema version of this message structure.
        /// </summary>
        public int Version { get; set; } = 0;

        /// <summary>
        /// The unique identifier of this request.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The id of the package, lowercased.
        /// </summary>
        public string NormalizedPackageId { get; set; }

        /// <summary>
        /// The version of the package in a normalized form.
        /// </summary>
        public string NormalizedPackageVersion { get; set; }

        /// <summary>
        /// The standard NuGet short name for this target framework, lowercased.
        /// </summary>
        public string NormalizedFrameworkTarget { get; set; }
    }
}