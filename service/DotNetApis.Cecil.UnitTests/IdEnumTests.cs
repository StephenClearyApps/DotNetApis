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
            var code = @"public enum SampleEnum { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:SampleEnum", type.XmldocIdentifier());
            Assert.Equal("SampleEnum", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleEnum", name.SimpleName);
            Assert.Equal("SampleEnum", name.QualifiedName);
            Assert.Equal("SampleEnum", name.FullyQualifiedName);
        }

        [Fact]
        public void InSingleNamespace()
        {
            var code = @"namespace MyNamespace { public enum SampleEnum { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:MyNamespace.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleEnum", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleEnum", name.SimpleName);
            Assert.Equal("MyNamespace.SampleEnum", name.QualifiedName);
            Assert.Equal("MyNamespace.SampleEnum", name.FullyQualifiedName);
        }

        [Fact]
        public void InMultipleNamespaces()
        {
            var code = @"namespace MyNamespace.InnerNamespace.ThirdNamespace { public enum SampleEnum { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:MyNamespace.InnerNamespace.ThirdNamespace.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleEnum", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleEnum", name.SimpleName);
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleEnum", name.QualifiedName);
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleEnum", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested()
        {
            var code = @"public class OuterClass { public enum SampleEnum { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:OuterClass.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("OuterClass/SampleEnum", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterClass.SampleEnum", name.SimpleName);
            Assert.Equal("OuterClass.SampleEnum", name.QualifiedName);
            Assert.Equal("OuterClass.SampleEnum", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_InNamespace()
        {
            var code = @"namespace Ns { public class OuterClass { public enum SampleEnum { } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:Ns.OuterClass.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass/SampleEnum", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterClass.SampleEnum", name.SimpleName);
            Assert.Equal("Ns.OuterClass.SampleEnum", name.QualifiedName);
            Assert.Equal("Ns.OuterClass.SampleEnum", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParameters()
        {
            var code = @"public struct OuterClass<TFirst, TSecond> { public enum SampleEnum { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:OuterClass`2.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("OuterClass'2/SampleEnum", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterClass<TFirst,TSecond>.SampleEnum", name.SimpleName);
            Assert.Equal("OuterClass<TFirst,TSecond>.SampleEnum", name.QualifiedName);
            Assert.Equal("OuterClass<TFirst,TSecond>.SampleEnum", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParameters_InNamespace()
        {
            var code = @"namespace Ns { public struct OuterClass<TFirst, TSecond> { public enum SampleEnum { } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleEnum");
            Assert.Equal("T:Ns.OuterClass`2.SampleEnum", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass'2/SampleEnum", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterClass<TFirst,TSecond>.SampleEnum", name.SimpleName);
            Assert.Equal("Ns.OuterClass<TFirst,TSecond>.SampleEnum", name.QualifiedName);
            Assert.Equal("Ns.OuterClass<TFirst,TSecond>.SampleEnum", name.FullyQualifiedName);
        }
    }
}
