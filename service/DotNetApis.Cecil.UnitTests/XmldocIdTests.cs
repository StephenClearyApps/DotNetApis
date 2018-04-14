using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class XmldocIdTests
    {
        [Fact]
        public void PublicClass_TopLevel()
        {
            var code = @"public class SampleClass { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.Equal("T:SampleClass", type.XmldocIdentifier());
        }
    }
}
