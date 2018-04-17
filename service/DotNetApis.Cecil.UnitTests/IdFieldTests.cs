using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class IdFieldTests
    {
        [Fact]
        public void Basic_InTopLevelType()
        {
            var code = @"public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int SampleField; }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var field = type.Fields.Single(x => x.Name == "SampleField");
            Assert.Equal("F:SampleClass.SampleField", field.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleField", field.DnaId());
            xmldoc.AssertXmldoc("Text to find.", field);
            field.MemberFriendlyName().AssertEqual("SampleField", "SampleClass.SampleField", "SampleClass.SampleField");
        }

        [Fact]
        public void Basic_InNamespacedType()
        {
            var code = @"namespace MyNamespace { public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int SampleField; } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var field = type.Fields.Single(x => x.Name == "SampleField");
            Assert.Equal("F:MyNamespace.SampleClass.SampleField", field.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleClass/SampleField", field.DnaId());
            xmldoc.AssertXmldoc("Text to find.", field);
            field.MemberFriendlyName().AssertEqual("SampleField", "SampleClass.SampleField", "MyNamespace.SampleClass.SampleField");
        }

        [Fact]
        public void Nested()
        {
            var code = @"public class OuterClass { public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int SampleField; } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            var field = type.Fields.Single(x => x.Name == "SampleField");
            Assert.Equal("F:OuterClass.SampleClass.SampleField", field.XmldocIdentifier());
            Assert.Equal("OuterClass/SampleClass/SampleField", field.DnaId());
            xmldoc.AssertXmldoc("Text to find.", field);
            field.MemberFriendlyName().AssertEqual("SampleField", "OuterClass.SampleClass.SampleField", "OuterClass.SampleClass.SampleField");
        }

        [Fact]
        public void Nested_InNamespace()
        {
            var code = @"namespace Ns { public class OuterClass { public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int SampleField; } } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            var field = type.Fields.Single(x => x.Name == "SampleField");
            Assert.Equal("F:Ns.OuterClass.SampleClass.SampleField", field.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass/SampleClass/SampleField", field.DnaId());
            xmldoc.AssertXmldoc("Text to find.", field);
            field.MemberFriendlyName().AssertEqual("SampleField", "OuterClass.SampleClass.SampleField", "Ns.OuterClass.SampleClass.SampleField");
        }

        [Fact]
        public void Nested_GenericParametersOnDeclaringType()
        {
            var code = @"public class SampleClass<TFirst> {
                        /// <summary>Text to find.</summary>
                        public int SampleField; }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`1");
            var field = type.Fields.Single(x => x.Name == "SampleField");
            Assert.Equal("F:SampleClass`1.SampleField", field.XmldocIdentifier());
            Assert.Equal("SampleClass'1/SampleField", field.DnaId());
            xmldoc.AssertXmldoc("Text to find.", field);
            field.MemberFriendlyName().AssertEqual("SampleField", "SampleClass<TFirst>.SampleField", "SampleClass<TFirst>.SampleField");
        }
    }
}
