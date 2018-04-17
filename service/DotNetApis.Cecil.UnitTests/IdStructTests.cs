using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class IdStructTests
    {
        [Fact]
        public void TopLevel()
        {
            var code = @"/// <summary>Text to find.</summary>
                        public struct SampleStruct { }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:SampleStruct", type.XmldocIdentifier());
            Assert.Equal("SampleStruct", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("SampleStruct", "SampleStruct", "SampleStruct");
        }

        [Fact]
        public void InSingleNamespace()
        {
            var code = @"namespace MyNamespace {
                        /// <summary>Text to find.</summary>
                        public struct SampleStruct { } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:MyNamespace.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleStruct", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("SampleStruct", "MyNamespace.SampleStruct", "MyNamespace.SampleStruct");
        }

        [Fact]
        public void InMultipleNamespaces()
        {
            var code = @"namespace MyNamespace.InnerNamespace.ThirdNamespace {
                        /// <summary>Text to find.</summary>
                        public struct SampleStruct { } }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:MyNamespace.InnerNamespace.ThirdNamespace.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleStruct", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("SampleStruct", "MyNamespace.InnerNamespace.ThirdNamespace.SampleStruct", "MyNamespace.InnerNamespace.ThirdNamespace.SampleStruct");
        }

        [Fact]
        public void Nested()
        {
            var code = @"public struct OuterStruct {
                        /// <summary>Text to find.</summary>
                        public struct SampleStruct { } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:OuterStruct.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("OuterStruct/SampleStruct", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterStruct.SampleStruct", "OuterStruct.SampleStruct", "OuterStruct.SampleStruct");
        }

        [Fact]
        public void Nested_InNamespace()
        {
            var code = @"namespace Ns { public struct OuterStruct {
                        /// <summary>Text to find.</summary>
                        public struct SampleStruct { } } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:Ns.OuterStruct.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterStruct/SampleStruct", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterStruct.SampleStruct", "Ns.OuterStruct.SampleStruct", "Ns.OuterStruct.SampleStruct");
        }

        [Fact]
        public void SingleGenericParameter()
        {
            var code = @"/// <summary>Text to find.</summary>
                        public struct SampleStruct<TFirst> { }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct`1");
            Assert.Equal("T:SampleStruct`1", type.XmldocIdentifier());
            Assert.Equal("SampleStruct'1", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("SampleStruct<TFirst>", "SampleStruct<TFirst>", "SampleStruct<TFirst>");
        }

        [Fact]
        public void MultipleGenericParameters()
        {
            var code = @"/// <summary>Text to find.</summary>
                        public struct SampleStruct<TFirst, TSecond> { }";
            var (assembly, xmldoc) = Compile(code);
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct`2");
            Assert.Equal("T:SampleStruct`2", type.XmldocIdentifier());
            Assert.Equal("SampleStruct'2", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("SampleStruct<TFirst,TSecond>", "SampleStruct<TFirst,TSecond>", "SampleStruct<TFirst,TSecond>");
        }

        [Fact]
        public void Nested_GenericParameters()
        {
            var code = @"public struct OuterStruct<TFirst, TSecond> {
                        /// <summary>Text to find.</summary>
                        public struct SampleStruct<TThird> { } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct`1");
            Assert.Equal("T:OuterStruct`2.SampleStruct`1", type.XmldocIdentifier());
            Assert.Equal("OuterStruct'2/SampleStruct'1", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", "OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", "OuterStruct<TFirst,TSecond>.SampleStruct<TThird>");
        }

        [Fact]
        public void Nested_GenericParameters_InNamespace()
        {
            var code = @"namespace Ns { public struct OuterStruct<TFirst, TSecond> {
                        /// <summary>Text to find.</summary>
                        public struct SampleStruct<TThird> { } } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct`1");
            Assert.Equal("T:Ns.OuterStruct`2.SampleStruct`1", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterStruct'2/SampleStruct'1", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", "Ns.OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", "Ns.OuterStruct<TFirst,TSecond>.SampleStruct<TThird>");
        }

        [Fact]
        public void Nested_GenericParametersOnlyOnOuter()
        {
            var code = @"public struct OuterStruct<TFirst, TSecond> {
                        /// <summary>Text to find.</summary>
                        public struct SampleStruct { } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:OuterStruct`2.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("OuterStruct'2/SampleStruct", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterStruct<TFirst,TSecond>.SampleStruct", "OuterStruct<TFirst,TSecond>.SampleStruct", "OuterStruct<TFirst,TSecond>.SampleStruct");
        }

        [Fact]
        public void Nested_GenericParameters_OnlyOnInner()
        {
            var code = @"public struct OuterStruct {
                        /// <summary>Text to find.</summary>
                        public struct SampleStruct<TThird> { } }";
            var (assembly, xmldoc) = Compile(code);
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct`1");
            Assert.Equal("T:OuterStruct.SampleStruct`1", type.XmldocIdentifier());
            Assert.Equal("OuterStruct/SampleStruct'1", type.DnaId());
            xmldoc.AssertXmldoc("Text to find.", type);
            type.MemberFriendlyName().AssertEqual("OuterStruct.SampleStruct<TThird>", "OuterStruct.SampleStruct<TThird>", "OuterStruct.SampleStruct<TThird>");
        }
    }
}
