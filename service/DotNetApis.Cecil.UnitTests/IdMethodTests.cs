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
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
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
            Assert.Equal("O:MyNamespace.SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("MyNamespace.SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "MyNamespace.SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "MyNamespace.SampleClass.SampleMethod");
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
            Assert.Equal("O:OuterClass.SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("OuterClass/SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "OuterClass.SampleClass.SampleMethod", "OuterClass.SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "OuterClass.SampleClass.SampleMethod", "OuterClass.SampleClass.SampleMethod");
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
            Assert.Equal("O:Ns.OuterClass.SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("Ns.OuterClass/SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "OuterClass.SampleClass.SampleMethod", "Ns.OuterClass.SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "OuterClass.SampleClass.SampleMethod", "Ns.OuterClass.SampleClass.SampleMethod");
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
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod<TFirst>", "SampleClass.SampleMethod<TFirst>", "SampleClass.SampleMethod<TFirst>");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
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
            Assert.Equal("O:OuterClass`2.SampleClass`1.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("OuterClass'2/SampleClass'1/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod<TFourth>", "OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod<TFourth>", "OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod<TFourth>");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod", "OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod");
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
            Assert.Equal("O:Ns.OuterClass`2.SampleClass`1.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("Ns.OuterClass'2/SampleClass'1/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod<TFourth>", "OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod<TFourth>", "Ns.OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod<TFourth>");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod", "Ns.OuterClass<TFirst,TSecond>.SampleClass<TThird>.SampleMethod");
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
            Assert.Equal("O:SampleClass`1.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass'1/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass<TFirst>.SampleMethod", "SampleClass<TFirst>.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass<TFirst>.SampleMethod", "SampleClass<TFirst>.SampleMethod");
        }

        [Fact]
        public void SingleParameter()
        {
            var code = @"public class SampleClass { public void SampleMethod(int x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void MultipleParameters()
        {
            var code = @"public class SampleClass { public void SampleMethod(int x, object y) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32,System.Object)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32,System.Object)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }
    }
}
