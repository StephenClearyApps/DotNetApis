using DotNetApis.Cecil;
using System;
using System.Linq;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class IsExposedTests
    {
        [Fact]
        public void PublicClass_IsExposed()
        {
            var code = @"public class SampleClass { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.True(type.IsExposed());
        }
    }
}
