using DotNetApis.Structure.Entities;
using DotNetApis.Structure.TypeReferences;
using System;
using System.Linq;
using Xunit;
using static DotNetApis.Logic.UnitTests.FormattingHelpers.FormatUtility;

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
            var type = structure.Types.OfType<TypeEntity>().Single(x => x.Name == "SampleClass");
            var method = type.Members.Instance.OfType<MethodEntity>().Single(x => x.Name == "SimpleMethod");

            // The method
            Assert.Equal(EntityKind.Method, method.Kind);
            Assert.Equal(EntityAccessibility.Public, method.Accessibility);
            Assert.Equal(EntityModifiers.None, method.Modifiers);
            Assert.Equal("SampleClass/SimpleMethod()", method.DnaId);
            Assert.Contains("This is my method. It does something.", method.Xmldoc.Basic.ToXmlString());

            // Return type
            var returnType = (KeywordTypeReference)method.ReturnType;
            Assert.Equal("void", returnType.Name);
        }
    }
}
