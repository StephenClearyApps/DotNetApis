using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetApis.Common.LogStructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetApis.Common
{
	/// <summary>
	/// A logger that writes messages to a <see cref="JsonWriter"/>.
	/// </summary>
	public sealed class JsonLoggerProvider : ILoggerProvider
	{
		private readonly ActionBlock<LogMessage> _queue;
		private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);
		private readonly JsonSerializer _serializer = JsonSerializer.Create(Constants.StorageJsonSerializerSettings);
		private JsonWriter _jsonWriter;

		public JsonLoggerProvider()
		{
			_queue = new ActionBlock<LogMessage>(ProcessMessage);
			_mutex.Wait();
		}

		private async Task ProcessMessage(LogMessage message)
		{
			await _mutex.WaitAsync().ConfigureAwait(false);
			try
			{
				_serializer.Serialize(_jsonWriter, message);
			}
			finally
			{
				_mutex.Release();
			}
		}

		void IDisposable.Dispose() { }

		public void Start(JsonWriter jsonWriter)
		{
			_jsonWriter = jsonWriter;
			_jsonWriter.WriteStartArray();
			_mutex.Release();
		}

		public async Task StopAsync()
		{
			_queue.Complete();
			await _queue.Completion.ConfigureAwait(false);
			_jsonWriter.WriteEndArray();
		}

		public ILogger CreateLogger(string categoryName) => new Logger(this, categoryName);

		private void Log<TState>(string categoryName, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			var message = new LogMessage
			{
				Type = logLevel,
				Timestamp = DateTimeOffset.UtcNow,
				Message = formatter(state, exception) + (exception == null ? "" : "\r\n" + exception),
				Category = categoryName,
				EventId = eventId.Id,
			};

			_queue.Post(message);
		}

		private sealed class Logger : ILogger
		{
			private readonly JsonLoggerProvider _provider;
			private readonly string _categoryName;

			public Logger(JsonLoggerProvider provider, string categoryName)
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
