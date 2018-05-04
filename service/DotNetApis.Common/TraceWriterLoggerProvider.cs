using System;
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DotNetApis.Common
{
	/// <summary>
	/// A logger provider that writes to a <see cref="TraceWriter"/>.
	/// </summary>
	public sealed class TraceWriterLoggerProvider : ILoggerProvider
	{
		private readonly TraceWriter _writer;

		public TraceWriterLoggerProvider(TraceWriter writer)
		{
			_writer = writer;
		}

		void IDisposable.Dispose() { }

		public ILogger CreateLogger(string categoryName) => new TraceWriterLogger(_writer, categoryName);

		private sealed class TraceWriterLogger : ILogger
		{
			private readonly TraceWriter _writer;
			private readonly string _categoryName;

			public TraceWriterLogger(TraceWriter writer, string categoryName)
			{
				_writer = writer;
				_categoryName = categoryName;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
				_writer.Trace(new TraceEvent(Map(logLevel), formatter(state, exception), _categoryName, exception));

			public bool IsEnabled(LogLevel logLevel) => true;

			public IDisposable BeginScope<TState>(TState state)
			{
				throw new NotImplementedException();
			}

			private static TraceLevel Map(LogLevel level)
			{
				switch (level)
				{
					case LogLevel.Trace:
						return TraceLevel.Verbose;
					case LogLevel.Information:
					case LogLevel.Debug:
						return TraceLevel.Info;
					case LogLevel.Warning:
						return TraceLevel.Warning;
					default:
						return TraceLevel.Error;
				}
			}
		}
	}
}
