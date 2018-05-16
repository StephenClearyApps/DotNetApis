using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetApis.Storage
{
    public interface IJsonBlobWriter: IBlobWriter
    {
        JsonWriter JsonWriter { get; }
    }
}
