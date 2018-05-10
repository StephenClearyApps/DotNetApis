using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Storage
{
    public interface IJsonBlobWriter
    {
	    Task CommitAsync();
		Uri Uri { get; }
		JsonWriter JsonWriter { get; }
    }
}
