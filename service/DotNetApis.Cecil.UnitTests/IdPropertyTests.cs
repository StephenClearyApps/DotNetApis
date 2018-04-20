using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class IdPropertyTests
    {
        [Fact]
        public void Basic_InTopLevelType()
        {
            var code = @"public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int SampleProperty { get; set; } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var prop = type.Properties.Single(x => x.Name == "SampleProperty");
            Assert.Equal("P:SampleClass.SampleProperty", prop.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleProperty", prop.DnaId());
            xmldoc.AssertXmldoc("Text to find.", prop);
            prop.MemberFriendlyName().AssertEqual("SampleProperty", "SampleClass.SampleProperty", "SampleClass.SampleProperty");
        }

        [Fact]
        public void Basic_InNamespacedType()
        {
            var code = @"namespace MyNamespace { public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int SampleProperty { get; set; } } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var prop = type.Properties.Single(x => x.Name == "SampleProperty");
            Assert.Equal("P:MyNamespace.SampleClass.SampleProperty", prop.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleClass/SampleProperty", prop.DnaId());
            xmldoc.AssertXmldoc("Text to find.", prop);
            prop.MemberFriendlyName().AssertEqual("SampleProperty", "SampleClass.SampleProperty", "MyNamespace.SampleClass.SampleProperty");
        }

        [Fact]
        public void Nested()
        {
            var code = @"public class OuterClass { public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int SampleProperty { get; set; } } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            var prop = type.Properties.Single(x => x.Name == "SampleProperty");
            Assert.Equal("P:OuterClass.SampleClass.SampleProperty", prop.XmldocIdentifier());
            Assert.Equal("OuterClass/SampleClass/SampleProperty", prop.DnaId());
            xmldoc.AssertXmldoc("Text to find.", prop);
            prop.MemberFriendlyName().AssertEqual("SampleProperty", "OuterClass.SampleClass.SampleProperty", "OuterClass.SampleClass.SampleProperty");
        }

        [Fact]
        public void Nested_InNamespace()
        {
            var code = @"namespace Ns { public class OuterClass { public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int SampleProperty { get; set; } } } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            var prop = type.Properties.Single(x => x.Name == "SampleProperty");
            Assert.Equal("P:Ns.OuterClass.SampleClass.SampleProperty", prop.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass/SampleClass/SampleProperty", prop.DnaId());
            xmldoc.AssertXmldoc("Text to find.", prop);
            prop.MemberFriendlyName().AssertEqual("SampleProperty", "OuterClass.SampleClass.SampleProperty", "Ns.OuterClass.SampleClass.SampleProperty");
        }

        [Fact]
        public void Nested_GenericParametersOnDeclaringType()
        {
            var code = @"public class SampleClass<TFirst> {
                        /// <summary>Text to find.</summary>
                        public int SampleProperty { get; set; } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`1");
            var prop = type.Properties.Single(x => x.Name == "SampleProperty");
            Assert.Equal("P:SampleClass`1.SampleProperty", prop.XmldocIdentifier());
            Assert.Equal("SampleClass'1/SampleProperty", prop.DnaId());
            xmldoc.AssertXmldoc("Text to find.", prop);
            prop.MemberFriendlyName().AssertEqual("SampleProperty", "SampleClass<TFirst>.SampleProperty", "SampleClass<TFirst>.SampleProperty");
        }

        [Fact]
        public void Indexer_SingleParameter()
        {
            var code = @"public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int this[int index] { get => 0; set { } } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var prop = type.Properties.Single(x => x.Name == "Item");
            Assert.Equal("P:SampleClass.Item(System.Int32)", prop.XmldocIdentifier());
            Assert.Equal("SampleClass/Item(System.Int32)", prop.DnaId());
            xmldoc.AssertXmldoc("Text to find.", prop);
            prop.MemberFriendlyName().AssertEqual("Item", "SampleClass.Item", "SampleClass.Item");
        }

        [Fact]
        public void Indexer_MultipleParameters()
        {
            var code = @"public class SampleClass {
                        /// <summary>Text to find.</summary>
                        public int this[int index, string str] { get => 0; set { } } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var prop = type.Properties.Single(x => x.Name == "Item");
            Assert.Equal("P:SampleClass.Item(System.Int32,System.String)", prop.XmldocIdentifier());
            Assert.Equal("SampleClass/Item(System.Int32,System.String)", prop.DnaId());
            xmldoc.AssertXmldoc("Text to find.", prop);
            prop.MemberFriendlyName().AssertEqual("Item", "SampleClass.Item", "SampleClass.Item");
        }
    }
}
