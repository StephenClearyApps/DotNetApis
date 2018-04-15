using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class IdClassTests
    {
        [Fact]
        public void TopLevel()
        {
            var code = @"public class SampleClass { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.Equal("T:SampleClass", type.XmldocIdentifier());
            Assert.Equal("SampleClass", type.DnaId());
            type.MemberFriendlyName().AssertEqual("SampleClass", "SampleClass", "SampleClass");
        }

        [Fact]
        public void InSingleNamespace()
        {
            var code = @"namespace MyNamespace { public class SampleClass { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.Equal("T:MyNamespace.SampleClass", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleClass", type.DnaId());
            type.MemberFriendlyName().AssertEqual("SampleClass", "MyNamespace.SampleClass", "MyNamespace.SampleClass");
        }

        [Fact]
        public void InMultipleNamespaces()
        {
            var code = @"namespace MyNamespace.InnerNamespace.ThirdNamespace { public class SampleClass { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.Equal("T:MyNamespace.InnerNamespace.ThirdNamespace.SampleClass", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleClass", type.DnaId());
            type.MemberFriendlyName().AssertEqual("SampleClass", "MyNamespace.InnerNamespace.ThirdNamespace.SampleClass", "MyNamespace.InnerNamespace.ThirdNamespace.SampleClass");
        }

        [Fact]
        public void Nested()
        {
            var code = @"public class OuterClass { public class SampleClass { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            Assert.Equal("T:OuterClass.SampleClass", type.XmldocIdentifier());
            Assert.Equal("OuterClass/SampleClass", type.DnaId());
            type.MemberFriendlyName().AssertEqual("OuterClass.SampleClass", "OuterClass.SampleClass", "OuterClass.SampleClass");
        }

        [Fact]
        public void Nested_InNamespace()
        {
            var code = @"namespace Ns { public class OuterClass { public class SampleClass { } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            Assert.Equal("T:Ns.OuterClass.SampleClass", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass/SampleClass", type.DnaId());
            type.MemberFriendlyName().AssertEqual("OuterClass.SampleClass", "Ns.OuterClass.SampleClass", "Ns.OuterClass.SampleClass");
        }

        [Fact]
        public void SingleGenericParameter()
        {
            var code = @"public class SampleClass<TFirst> { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`1");
            Assert.Equal("T:SampleClass`1", type.XmldocIdentifier());
            Assert.Equal("SampleClass'1", type.DnaId());
            type.MemberFriendlyName().AssertEqual("SampleClass<TFirst>", "SampleClass<TFirst>", "SampleClass<TFirst>");
        }

        [Fact]
        public void MultipleGenericParameters()
        {
            var code = @"public class SampleClass<TFirst, TSecond> { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`2");
            Assert.Equal("T:SampleClass`2", type.XmldocIdentifier());
            Assert.Equal("SampleClass'2", type.DnaId());
            type.MemberFriendlyName().AssertEqual("SampleClass<TFirst,TSecond>", "SampleClass<TFirst,TSecond>", "SampleClass<TFirst,TSecond>");
        }

        [Fact]
        public void Nested_GenericParameters()
        {
            var code = @"public class OuterClass<TFirst, TSecond> { public class SampleClass<TThird> { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass`1");
            Assert.Equal("T:OuterClass`2.SampleClass`1", type.XmldocIdentifier());
            Assert.Equal("OuterClass'2/SampleClass'1", type.DnaId());
            type.MemberFriendlyName().AssertEqual("OuterClass<TFirst,TSecond>.SampleClass<TThird>", "OuterClass<TFirst,TSecond>.SampleClass<TThird>", "OuterClass<TFirst,TSecond>.SampleClass<TThird>");
        }

        [Fact]
        public void Nested_GenericParameters_InNamespace()
        {
            var code = @"namespace Ns { public class OuterClass<TFirst, TSecond> { public class SampleClass<TThird> { } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass`1");
            Assert.Equal("T:Ns.OuterClass`2.SampleClass`1", type.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass'2/SampleClass'1", type.DnaId());
            type.MemberFriendlyName().AssertEqual("OuterClass<TFirst,TSecond>.SampleClass<TThird>", "Ns.OuterClass<TFirst,TSecond>.SampleClass<TThird>", "Ns.OuterClass<TFirst,TSecond>.SampleClass<TThird>");
        }

        [Fact]
        public void Nested_GenericParametersOnlyOnOuter()
        {
            var code = @"public class OuterClass<TFirst, TSecond> { public class SampleClass { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            Assert.Equal("T:OuterClass`2.SampleClass", type.XmldocIdentifier());
            Assert.Equal("OuterClass'2/SampleClass", type.DnaId());
            type.MemberFriendlyName().AssertEqual("OuterClass<TFirst,TSecond>.SampleClass", "OuterClass<TFirst,TSecond>.SampleClass", "OuterClass<TFirst,TSecond>.SampleClass");
        }

        [Fact]
        public void Nested_GenericParameters_OnlyOnInner()
        {
            var code = @"public class OuterClass { public class SampleClass<TThird> { } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass`1");
            Assert.Equal("T:OuterClass.SampleClass`1", type.XmldocIdentifier());
            Assert.Equal("OuterClass/SampleClass'1", type.DnaId());
            type.MemberFriendlyName().AssertEqual("OuterClass.SampleClass<TThird>", "OuterClass.SampleClass<TThird>", "OuterClass.SampleClass<TThird>");
        }
    }
}
