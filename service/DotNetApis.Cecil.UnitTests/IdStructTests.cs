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
            var code = @"public struct SampleStruct { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:SampleStruct", type.XmldocIdentifier());
            Assert.Equal("SampleStruct", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleStruct", name.SimpleName);
            Assert.Equal("SampleStruct", name.QualifiedName);
            Assert.Equal("SampleStruct", name.FullyQualifiedName);
        }

        [Fact]
        public void InSingleNamespace()
        {
            var code = @"namespace MyNamespace { public struct SampleStruct { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:MyNamespace.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleStruct", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleStruct", name.SimpleName);
            Assert.Equal("MyNamespace.SampleStruct", name.QualifiedName);
            Assert.Equal("MyNamespace.SampleStruct", name.FullyQualifiedName);
        }

        [Fact]
        public void InMultipleNamespaces()
        {
            var code = @"namespace MyNamespace.InnerNamespace.ThirdNamespace { public struct SampleStruct { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:MyNamespace.InnerNamespace.ThirdNamespace.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleStruct", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleStruct", name.SimpleName);
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleStruct", name.QualifiedName);
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleStruct", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested()
        {
            var code = @"public struct OuterStruct { public struct SampleStruct { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:OuterStruct.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("OuterStruct/SampleStruct", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterStruct.SampleStruct", name.SimpleName);
            Assert.Equal("OuterStruct.SampleStruct", name.QualifiedName);
            Assert.Equal("OuterStruct.SampleStruct", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_InNamespace()
        {
            var code = @"namespace Ns { public struct OuterStruct { public struct SampleStruct { } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:Ns.OuterStruct.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterStruct/SampleStruct", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterStruct.SampleStruct", name.SimpleName);
            Assert.Equal("Ns.OuterStruct.SampleStruct", name.QualifiedName);
            Assert.Equal("Ns.OuterStruct.SampleStruct", name.FullyQualifiedName);
        }

        [Fact]
        public void SingleGenericParameter()
        {
            var code = @"public struct SampleStruct<TFirst> { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct`1");
            Assert.Equal("T:SampleStruct`1", type.XmldocIdentifier());
            Assert.Equal("SampleStruct'1", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleStruct<TFirst>", name.SimpleName);
            Assert.Equal("SampleStruct<TFirst>", name.QualifiedName);
            Assert.Equal("SampleStruct<TFirst>", name.FullyQualifiedName);
        }

        [Fact]
        public void MultipleGenericParameters()
        {
            var code = @"public struct SampleStruct<TFirst, TSecond> { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleStruct`2");
            Assert.Equal("T:SampleStruct`2", type.XmldocIdentifier());
            Assert.Equal("SampleStruct'2", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleStruct<TFirst,TSecond>", name.SimpleName);
            Assert.Equal("SampleStruct<TFirst,TSecond>", name.QualifiedName);
            Assert.Equal("SampleStruct<TFirst,TSecond>", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParameters()
        {
            var code = @"public struct OuterStruct<TFirst, TSecond> { public struct SampleStruct<TThird> { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct`1");
            Assert.Equal("T:OuterStruct`2.SampleStruct`1", type.XmldocIdentifier());
            Assert.Equal("OuterStruct'2/SampleStruct'1", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", name.SimpleName);
            Assert.Equal("OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", name.QualifiedName);
            Assert.Equal("OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParameters_InNamespace()
        {
            var code = @"namespace Ns { public struct OuterStruct<TFirst, TSecond> { public struct SampleStruct<TThird> { } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct`1");
            Assert.Equal("T:Ns.OuterStruct`2.SampleStruct`1", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterStruct'2/SampleStruct'1", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", name.SimpleName);
            Assert.Equal("Ns.OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", name.QualifiedName);
            Assert.Equal("Ns.OuterStruct<TFirst,TSecond>.SampleStruct<TThird>", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParametersOnlyOnOuter()
        {
            var code = @"public struct OuterStruct<TFirst, TSecond> { public struct SampleStruct { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct");
            Assert.Equal("T:OuterStruct`2.SampleStruct", type.XmldocIdentifier());
            Assert.Equal("OuterStruct'2/SampleStruct", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterStruct<TFirst,TSecond>.SampleStruct", name.SimpleName);
            Assert.Equal("OuterStruct<TFirst,TSecond>.SampleStruct", name.QualifiedName);
            Assert.Equal("OuterStruct<TFirst,TSecond>.SampleStruct", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParameters_OnlyOnInner()
        {
            var code = @"public struct OuterStruct { public struct SampleStruct<TThird> { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterStruct");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleStruct`1");
            Assert.Equal("T:OuterStruct.SampleStruct`1", type.XmldocIdentifier());
            Assert.Equal("OuterStruct/SampleStruct'1", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterStruct.SampleStruct<TThird>", name.SimpleName);
            Assert.Equal("OuterStruct.SampleStruct<TThird>", name.QualifiedName);
            Assert.Equal("OuterStruct.SampleStruct<TThird>", name.FullyQualifiedName);
        }
    }
}
