using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetApis.Common;
using DotNetApis.Storage;
using Newtonsoft.Json;

namespace DotNetApis.Logic
{
    public sealed class StreamingJsonWriter: IBlobWriter
    {
	    private readonly IJsonBlobWriter _writer;
	    private readonly ActionBlock<Action> _queue;
	    private readonly JsonSerializer _serializer;

	    public StreamingJsonWriter(IJsonBlobWriter writer)
	    {
		    _writer = writer;
			_queue = new ActionBlock<Action>(x => x());
			_serializer = JsonSerializer.Create(Constants.StorageJsonSerializerSettings);
	    }

	    public Uri Uri => _writer.Uri;

	    public async Task CommitAsync()
	    {
		    _queue.Complete();
		    await _queue.Completion.ConfigureAwait(false);
		    await _writer.CommitAsync().ConfigureAwait(false);
	    }

	    public void SerializeObject(object value) => _queue.Post(() => _serializer.Serialize(_writer.JsonWriter, value));
	    public void WriteStartObject() => _queue.Post(() => _writer.JsonWriter.WriteStartObject());
	    public void WriteEndObject() => _queue.Post(() => _writer.JsonWriter.WriteEndObject());
	    public void WriteStartArray() => _queue.Post(() => _writer.JsonWriter.WriteStartArray());
	    public void WriteEndArray() => _queue.Post(() => _writer.JsonWriter.WriteEndArray());
	    public void WritePropertyName(string name) => _queue.Post(() => _writer.JsonWriter.WritePropertyName(name));

	    public void WriteProperty(string name, object value)
	    {
		    if (value == null || value as string == "" || (value is ICollection arrayValue && arrayValue.Count == 0))
			    return;
		    _queue.Post(() =>
		    {
			    _writer.JsonWriter.WritePropertyName(name);
			    _serializer.Serialize(_writer.JsonWriter, value);
		    });
	    }
    }
}
