using DotNetApis.Structure.Entities;
using System;
using System.Linq;
using Xunit;
using static FormatUtility;

namespace DotNetApis.Logic.UnitTests
{
    public class FormattingClasses
    {
        [Fact]
        public void PublicClass_InNamespace()
        {
            var code = @"namespace Test {
                        /// <summary>
                        /// This is my sample class!
                        /// </summary>
                        public class SampleClass { } }";
            var structure = Format(code);
            Assert.Collection(structure.Types,
                type =>
                {
                    Assert.Equal(EntityKind.Class, type.Kind);
                    Assert.IsType<TypeEntity>(type);
                    Assert.Equal(EntityAccessibility.Public, type.Accessibility);
                    Assert.Equal(EntityModifiers.None, type.Modifiers);
                    Assert.Equal("SampleClass", type.Name);
                    Assert.Equal("Test.SampleClass", type.DnaId);
                    Assert.Contains("This is my sample class!", type.Xmldoc.Basic.ToXmlString());
                });
        }

        [Fact]
        public void PublicClass_TopLevel()
        {
            var code = @"/// <summary>
                        /// This is my sample class!
                        /// </summary>
                        public class SampleClass { }";
            var structure = Format(code);
            Assert.Collection(structure.Types,
                type =>
                {
                    Assert.Equal(EntityKind.Class, type.Kind);
                    Assert.IsType<TypeEntity>(type);
                    Assert.Equal(EntityAccessibility.Public, type.Accessibility);
                    Assert.Equal(EntityModifiers.None, type.Modifiers);
                    Assert.Equal("SampleClass", type.Name);
                    Assert.Equal("SampleClass", type.DnaId);
                    Assert.Contains("This is my sample class!", type.Xmldoc.Basic.ToXmlString());
                });
        }
    }
}
