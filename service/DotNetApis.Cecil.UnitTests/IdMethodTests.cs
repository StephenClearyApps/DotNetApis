using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class IdMethodTests
    {
        [Fact]
        public void Basic_InTopLevelType()
        {
            var code = @"public class SampleClass { public void SampleMethod() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod()", method.DnaId());
            var name = method.MemberFriendlyName();
            Assert.Equal("SampleMethod", name.SimpleName);
            Assert.Equal("SampleClass.SampleMethod", name.QualifiedName);
            Assert.Equal("SampleClass.SampleMethod", name.FullyQualifiedName);
        }

        [Fact]
        public void Basic_InNamespacedType()
        {
            var code = @"namespace MyNamespace { public class SampleClass { public void SampleMethod() { } } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:MyNamespace.SampleClass.SampleMethod", method.XmldocIdentifier());
            Assert.Equal("MyNamespace.SampleClass/SampleMethod()", method.DnaId());
            var name = method.MemberFriendlyName();
            Assert.Equal("SampleMethod", name.SimpleName);
            Assert.Equal("SampleClass.SampleMethod", name.QualifiedName);
            Assert.Equal("MyNamespace.SampleClass.SampleMethod", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested()
        {
            var code = @"public class OuterClass { public class SampleClass { public void SampleMethod() { } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:OuterClass.SampleClass.SampleMethod", method.XmldocIdentifier());
            Assert.Equal("OuterClass/SampleClass/SampleMethod()", method.DnaId());
            var name = method.MemberFriendlyName();
            Assert.Equal("SampleMethod", name.SimpleName);
            Assert.Equal("OuterClass.SampleClass.SampleMethod", name.QualifiedName);
            Assert.Equal("OuterClass.SampleClass.SampleMethod", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_InNamespace()
        {
            var code = @"namespace Ns { public class OuterClass { public class SampleClass { public void SampleMethod() { } } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:Ns.OuterClass.SampleClass.SampleMethod", method.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass/SampleClass/SampleMethod()", method.DnaId());
            var name = method.MemberFriendlyName();
            Assert.Equal("SampleMethod", name.SimpleName);
            Assert.Equal("OuterClass.SampleClass.SampleMethod", name.QualifiedName);
            Assert.Equal("Ns.OuterClass.SampleClass.SampleMethod", name.FullyQualifiedName);
        }

        [Fact]
        public void SingleGenericParameter()
        {
            var code = @"public class SampleClass { public void SampleMethod<TFirst>() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod``1", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod''1()", method.DnaId());
            var name = method.MemberFriendlyName();
            Assert.Equal("SampleMethod<TFirst>", name.SimpleName);
            Assert.Equal("SampleClass.SampleMethod<TFirst>", name.QualifiedName);
            Assert.Equal("SampleClass.SampleMethod<TFirst>", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParameters()
        {
            var code = @"public class OuterClass<TFirst, TSecond> { public class SampleClass<TThird> { public void SampleMethod<TFourth>() { } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass`1");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:OuterClass`2.SampleClass`1.SampleMethod``1", method.XmldocIdentifier());
            Assert.Equal("OuterClass'2/SampleClass'1/SampleMethod''1()", method.DnaId());
            var name = method.MemberFriendlyName();
            Assert.Equal("SampleMethod<TFourth>", name.SimpleName);
            Assert.Equal("OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod<TFourth>", name.QualifiedName);
            Assert.Equal("OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod<TFourth>", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParameters_InNamespace()
        {
            var code = @"namespace Ns { public class OuterClass<TFirst, TSecond> { public class SampleClass<TThird> { public void SampleMethod<TFourth>() { } } } }";
            var assembly = Compile(code).Dll;
            var outer = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "OuterClass`2");
            var type = outer.NestedTypes.Single(x => x.Name == "SampleClass`1");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:Ns.OuterClass`2.SampleClass`1.SampleMethod``1", method.XmldocIdentifier());
            Assert.Equal("Ns.OuterClass'2/SampleClass'1/SampleMethod''1()", method.DnaId());
            var name = method.MemberFriendlyName();
            Assert.Equal("SampleMethod<TFourth>", name.SimpleName);
            Assert.Equal("OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod<TFourth>", name.QualifiedName);
            Assert.Equal("Ns.OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod<TFourth>", name.FullyQualifiedName);
        }

        [Fact]
        public void Nested_GenericParametersOnlyOnDeclaringType()
        {
            var code = @"public class SampleClass<TFirst> { public void SampleMethod() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`1");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass`1.SampleMethod", method.XmldocIdentifier());
            Assert.Equal("SampleClass'1/SampleMethod()", method.DnaId());
            var name = method.MemberFriendlyName();
            Assert.Equal("SampleMethod", name.SimpleName);
            Assert.Equal("SampleClass<TFirst>.SampleMethod", name.QualifiedName);
            Assert.Equal("SampleClass<TFirst>.SampleMethod", name.FullyQualifiedName);
        }
    }
}
