using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetApis.Storage
{
    public interface IBlobWriter
    {
        Task CommitAsync();
        Uri Uri { get; }
    }
}
