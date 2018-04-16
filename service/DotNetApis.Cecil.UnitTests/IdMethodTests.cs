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

        [Fact]
        public void Generic_SingleParameter()
        {
            var code = @"public class SampleClass { public void SampleMethod<TFirst>(int x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod``1(System.Int32)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod''1(System.Int32)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod<TFirst>", "SampleClass.SampleMethod<TFirst>", "SampleClass.SampleMethod<TFirst>");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void Generic_SingleParameterOfGenericType()
        {
            var code = @"public class SampleClass { public void SampleMethod<TFirst>(TFirst x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod``1(``0)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod''1(''0)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod<TFirst>", "SampleClass.SampleMethod<TFirst>", "SampleClass.SampleMethod<TFirst>");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void NestedGeneric_WithParametersOfGenericTypes()
        {
            var code = @"public class SampleClass<TFirst, TSecond, TThird> { public void SampleMethod<TFourth>(TSecond x, TFourth y) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`3");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass`3.SampleMethod``1(`1,``0)", method.XmldocIdentifier());
            Assert.Equal("SampleClass'3/SampleMethod''1('1,''0)", method.DnaId());
            Assert.Equal("O:SampleClass`3.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass'3/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod<TFourth>", "SampleClass<TFirst,TSecond,TThird>.SampleMethod<TFourth>", "SampleClass<TFirst,TSecond,TThird>.SampleMethod<TFourth>");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass<TFirst,TSecond,TThird>.SampleMethod", "SampleClass<TFirst,TSecond,TThird>.SampleMethod");
        }

        [Fact]
        public void PointerParameter()
        {
            var code = @"public class SampleClass { public unsafe void SampleMethod(int * x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32*)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32~)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void RefParameter()
        {
            var code = @"public class SampleClass { public void SampleMethod(ref int x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32@)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32-)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void OutParameter()
        {
            var code = @"public class SampleClass { public void SampleMethod(out int x) { x = 0; } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32@)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32-)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void SimpleArrayParameter()
        {
            var code = @"public class SampleClass { public void SampleMethod(int[] x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32[])", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32$)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void PointerToPointer()
        {
            var code = @"public class SampleClass { public unsafe void SampleMethod(int ** x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32**)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32~~)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void ArrayOfPointers()
        {
            var code = @"public class SampleClass { public unsafe void SampleMethod(int*[] x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32*[])", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32~$)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void ArrayOfArrays()
        {
            var code = @"public class SampleClass { public void SampleMethod(int[][] x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32[][])", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32$$)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void MultidimensionalArray()
        {
            var code = @"public class SampleClass { public void SampleMethod(int[,] x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32[0:,0:])", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32@5B0;,0;@5D)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void ArrayOfPointersByRef()
        {
            var code = @"public class SampleClass { public unsafe void SampleMethod(ref int*[] x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Int32*[]@)", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Int32~$-)", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void ImplicitConversion()
        {
            var code = @"public class SampleClass { public static implicit operator int(SampleClass x) => 0; }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "op_Implicit");
            Assert.Equal("M:SampleClass.op_Implicit(SampleClass)~System.Int32", method.XmldocIdentifier());
            Assert.Equal("SampleClass/op_Implicit(SampleClass)~System.Int32", method.DnaId());
            Assert.Equal("O:SampleClass.op_Implicit", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/op_Implicit", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("op_Implicit", "SampleClass.op_Implicit", "SampleClass.op_Implicit");
            method.OverloadFriendlyName().AssertEqual("op_Implicit", "SampleClass.op_Implicit", "SampleClass.op_Implicit");
        }

        [Fact]
        public void SimpleGenericParameter()
        {
            var code = @"using System.Collections.Generic; public class SampleClass { public void SampleMethod(List<int> x) { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethod");
            Assert.Equal("M:SampleClass.SampleMethod(System.Collections.Generic.List{System.Int32})", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod(System.Collections.Generic.List(System.Int32))", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("SampleMethod", "SampleClass.SampleMethod", "SampleClass.SampleMethod");
        }

        [Fact]
        public void ExplicitInterfaceImplementation()
        {
            var code = @"using System; public class SampleClass: IDisposable { void IDisposable.Dispose() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "System.IDisposable.Dispose");
            Assert.Equal("M:SampleClass.System#IDisposable#Dispose", method.XmldocIdentifier());
            Assert.Equal("SampleClass/System.IDisposable.Dispose()", method.DnaId());
            Assert.Equal("O:SampleClass.System#IDisposable#Dispose", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/System.IDisposable.Dispose", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("System.IDisposable.Dispose", "SampleClass.System.IDisposable.Dispose", "SampleClass.System.IDisposable.Dispose");
            method.OverloadFriendlyName().AssertEqual("System.IDisposable.Dispose", "SampleClass.System.IDisposable.Dispose", "SampleClass.System.IDisposable.Dispose");
        }

        [Fact]
        public void ExplicitInterfaceImplementation_OfGenericInterface()
        {
            var code = @"public interface IGeneric<TFirst> { void SampleMethod(); } public class SampleClass<T>: IGeneric<T> { void IGeneric<T>.SampleMethod() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`1");
            var method = type.Methods.Single(x => x.Name == "IGeneric<T>.SampleMethod");
            Assert.Equal("M:SampleClass`1.IGeneric{T}#SampleMethod", method.XmldocIdentifier());
            Assert.Equal("SampleClass'1/IGeneric(T).SampleMethod()", method.DnaId());
            Assert.Equal("O:SampleClass`1.IGeneric{T}#SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass'1/IGeneric(T).SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("IGeneric<T>.SampleMethod", "SampleClass<T>.IGeneric<T>.SampleMethod", "SampleClass<T>.IGeneric<T>.SampleMethod");
            method.OverloadFriendlyName().AssertEqual("IGeneric<T>.SampleMethod", "SampleClass<T>.IGeneric<T>.SampleMethod", "SampleClass<T>.IGeneric<T>.SampleMethod");
        }

        [Fact]
        public void ExplicitInterfaceImplementation_OfGenericMethodOfGenericInterface()
        {
            var code = @"public interface IGeneric<TFirst> { void SampleMethod<TSecond>(); } public class SampleClass<T>: IGeneric<T> { void IGeneric<T>.SampleMethod<TThird>() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass`1");
            var method = type.Methods.Single(x => x.Name == "IGeneric<T>.SampleMethod");
            Assert.Equal("M:SampleClass`1.IGeneric{T}#SampleMethod``1", method.XmldocIdentifier());
            Assert.Equal("SampleClass'1/IGeneric(T).SampleMethod''1()", method.DnaId());
            Assert.Equal("O:SampleClass`1.IGeneric{T}#SampleMethod", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass'1/IGeneric(T).SampleMethod", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("IGeneric<T>.SampleMethod<TThird>", "SampleClass<T>.IGeneric<T>.SampleMethod<TThird>", "SampleClass<T>.IGeneric<T>.SampleMethod<TThird>");
            method.OverloadFriendlyName().AssertEqual("IGeneric<T>.SampleMethod", "SampleClass<T>.IGeneric<T>.SampleMethod", "SampleClass<T>.IGeneric<T>.SampleMethod");
        }

        [Fact]
        public void Constructor()
        {
            var code = @"public class SampleClass { public SampleClass() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == ".ctor");
            Assert.Equal("M:SampleClass.#ctor", method.XmldocIdentifier());
            Assert.Equal("SampleClass/.ctor()", method.DnaId());
            Assert.Equal("O:SampleClass.#ctor", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/.ctor", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual(".ctor", "SampleClass..ctor", "SampleClass..ctor"); // TODO: expected behavior?
            method.OverloadFriendlyName().AssertEqual(".ctor", "SampleClass..ctor", "SampleClass..ctor");
        }

        [Fact]
        public void EscapedCharacter()
        {
            var code = @"public class SampleClass { public void SampleMethôd() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "SampleMethôd");
            Assert.Equal("M:SampleClass.SampleMethôd", method.XmldocIdentifier());
            Assert.Equal("SampleClass/SampleMeth@C3@B4d()", method.DnaId());
            Assert.Equal("O:SampleClass.SampleMethôd", method.OverloadXmldocIdentifier());
            Assert.Equal("SampleClass/SampleMeth@C3@B4d", method.OverloadDnaId());
            method.MemberFriendlyName().AssertEqual("SampleMethôd", "SampleClass.SampleMethôd", "SampleClass.SampleMethôd");
            method.OverloadFriendlyName().AssertEqual("SampleMethôd", "SampleClass.SampleMethôd", "SampleClass.SampleMethôd");
        }
    }
}
