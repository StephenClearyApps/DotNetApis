using DotNetApis.Structure.Entities;
using System;
using System.Linq;
using Xunit;
using static FormatUtility;

namespace DotNetApis.Logic.UnitTests
{
    public class FormattingMethods
    {
        [Fact]
        public void PublicMethod()
        {
            var code = @"public class SampleClass
                        {
                            /// <summary>
                            /// This is my method. It does something.
                            /// </summary>
                            public void SimpleMethod() { }
                        }";
            var structure = Format(code);
            Assert.Collection(structure.Types,
                t =>
                {
                    var type = (TypeEntity)t;
                    Assert.Collection(type.Members.Instance, method =>
                    {
                        Assert.Equal(EntityKind.Method, method.Kind);
                        Assert.IsType<MethodEntity>(method);
                        Assert.Equal(EntityAccessibility.Public, method.Accessibility);
                        Assert.Equal(EntityModifiers.None, method.Modifiers);
                        Assert.Equal("SimpleMethod", method.Name);
                        Assert.Equal("SampleClass/SimpleMethod()", method.DnaId);
                        Assert.Contains("This is my method. It does something.", method.Xmldoc.Basic.ToXmlString());
                    });
                });
        }
    }
}
