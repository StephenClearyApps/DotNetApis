using System;
using System.Collections.Generic;
using DotNetApis.Common.LogStructure;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
	/// <summary>
	/// A logger that writes messages to a an in-memory list.
	/// </summary>
	public sealed class InMemoryLoggerProvider : ILoggerProvider
	{
		public List<LogMessage> Messages { get; } = new List<LogMessage>();

		void IDisposable.Dispose() { }

		public ILogger CreateLogger(string categoryName) => new InMemoryLogger(this, categoryName);

		private void Log<TState>(string categoryName, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			Messages.Add(new LogMessage
			{
				Type = logLevel,
				Timestamp = DateTimeOffset.UtcNow,
				Message = formatter(state, exception) + (exception == null ? "" : "\r\n" + exception),
				Category = categoryName,
				EventId = eventId.Id,
			});
		}

		private sealed class InMemoryLogger : ILogger
		{
			private readonly InMemoryLoggerProvider _provider;
			private readonly string _categoryName;

			public InMemoryLogger(InMemoryLoggerProvider provider, string categoryName)
			{
				_provider = provider;
				_categoryName = categoryName;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
				=> _provider.Log(_categoryName, logLevel, eventId, state, exception, formatter);

			public bool IsEnabled(LogLevel logLevel) => true;

			public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
		}
	}
}
