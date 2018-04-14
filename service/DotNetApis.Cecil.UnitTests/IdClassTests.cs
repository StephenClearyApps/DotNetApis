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
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleClass", name.SimpleName);
            Assert.Equal("SampleClass", name.QualifiedName);
            Assert.Equal("SampleClass", name.FullyQualifiedName);
        }

        [Fact]
        public void InSingleNamespace()
        {
            var code = @"namespace MyNamespace { public class SampleClass { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.Equal("T:MyNamespace.SampleClass", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleClass", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleClass", name.SimpleName);
            Assert.Equal("MyNamespace.SampleClass", name.QualifiedName);
            Assert.Equal("MyNamespace.SampleClass", name.FullyQualifiedName);
        }

        [Fact]
        public void InMultipleNamespaces()
        {
            var code = @"namespace MyNamespace.InnerNamespace.ThirdNamespace { public class SampleClass { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.Equal("T:MyNamespace.InnerNamespace.ThirdNamespace.SampleClass", type.XmldocIdentifier());
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleClass", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleClass", name.SimpleName);
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleClass", name.QualifiedName);
            Assert.Equal("MyNamespace.InnerNamespace.ThirdNamespace.SampleClass", name.FullyQualifiedName);
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
            var name = type.MemberFriendlyName();
            Assert.Equal("OuterClass.SampleClass", name.SimpleName);
            Assert.Equal("OuterClass.SampleClass", name.QualifiedName);
            Assert.Equal("OuterClass.SampleClass", name.FullyQualifiedName);
        }

        [Fact]
        public void SingleGenericParameter()
        {
            var code = @"public class SampleClass<TFirst> { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`1");
            Assert.Equal("T:SampleClass`1", type.XmldocIdentifier());
            Assert.Equal("SampleClass'1", type.DnaId());
            var name = type.MemberFriendlyName();
            Assert.Equal("SampleClass<TFirst>", name.SimpleName);
            Assert.Equal("SampleClass<TFirst>", name.QualifiedName);
            Assert.Equal("SampleClass<TFirst>", name.FullyQualifiedName);
        }
    }
}
