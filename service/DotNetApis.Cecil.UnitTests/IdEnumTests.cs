using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class IdEnumTests
    {
        [Fact]
        public void TopLevel()
        {
            var code = @"/// <summary>Text to find.</summary>
                        public enum SampleEnum { }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:SampleEnum", type.XmldocIdentifier());
            Assert.Equal("SampleEnum", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("SampleEnum", "SampleEnum", "SampleEnum");
        }

        [Fact]
        public void InSingleNamespace()
        {
            var code = @"namespace MyNamespace {
                        /// <summary>Text to find.</summary>
                        public enum SampleEnum { } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:MyNamespace.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleEnum", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("SampleEnum", "MyNamespace.SampleEnum", "MyNamespace.SampleEnum");
        }

        [Fact]
        public void InMultipleNamespaces()
        {
            var code = @"namespace MyNamespace.InnerNamespace.ThirdNamespace {
                        /// <summary>Text to find.</summary>
                        public enum SampleEnum { } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:MyNamespace.InnerNamespace.ThirdNamespace.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleEnum", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("SampleEnum", "MyNamespace.InnerNamespace.ThirdNamespace.SampleEnum", "MyNamespace.InnerNamespace.ThirdNamespace.SampleEnum");
        }

        [Fact]
        public void Nested()
        {
            var code = @"public class OuterClass {
                        /// <summary>Text to find.</summary>
                        public enum SampleEnum { } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:OuterClass.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("OuterClass/SampleEnum", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterClass.SampleEnum", "OuterClass.SampleEnum", "OuterClass.SampleEnum");
        }

        [Fact]
        public void Nested_InNamespace()
        {
            var code = @"namespace Ns { public class OuterClass {
                        /// <summary>Text to find.</summary>
                        public enum SampleEnum { } } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:Ns.OuterClass.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass/SampleEnum", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterClass.SampleEnum", "Ns.OuterClass.SampleEnum", "Ns.OuterClass.SampleEnum");
        }

        [Fact]
        public void Nested_GenericParameters()
        {
            var code = @"public struct OuterClass<TFirst, TSecond> {
                        /// <summary>Text to find.</summary>
                        public enum SampleEnum { } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:OuterClass`2.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("OuterClass'2/SampleEnum", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterClass<TFirst,TSecond>.SampleEnum", "OuterClass<TFirst,TSecond>.SampleEnum", "OuterClass<TFirst,TSecond>.SampleEnum");
        }

        [Fact]
        public void Nested_GenericParameters_InNamespace()
        {
            var code = @"namespace Ns { public struct OuterClass<TFirst, TSecond> {
                        /// <summary>Text to find.</summary>
                        public enum SampleEnum { } } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:Ns.OuterClass`2.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass'2/SampleEnum", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterClass<TFirst,TSecond>.SampleEnum", "Ns.OuterClass<TFirst,TSecond>.SampleEnum", "Ns.OuterClass<TFirst,TSecond>.SampleEnum");
        }
    }
}
