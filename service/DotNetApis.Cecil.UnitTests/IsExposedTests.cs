using DotNetApis.Cecil;
using System;
using System.Linq;
using Xunit;
using static Utility;

namespace DotNetApis.Cecil.UnitTests
{
    public class IsExposedTests
    {
        [Fact]
        public void PublicClass_IsExposed()
        {
            var code = @"public class SampleClass { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.True(type.IsExposed());
        }

        [Fact]
        public void InternalClass_IsNotExposed()
        {
            var code = @"internal class SampleClass { }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            Assert.False(type.IsExposed());
        }

        [Fact]
        public void PublicMethod_OfPublicClass_IsExposed()
        {
            var code = @"public class SampleClass { public void Member() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "Member");
            Assert.True(method.IsExposed());
            Assert.Contains(type.ExposedMembers(), x => x.Name == "Member");
        }

        [Fact]
        public void InternalMethod_OfPublicClass_IsNotExposed()
        {
            var code = @"public class SampleClass { internal void Member() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "Member");
            Assert.False(method.IsExposed());
            Assert.DoesNotContain(type.ExposedMembers(), x => x.Name == "Member");
        }

        [Fact]
        public void ProtectedMethod_OfPublicUnsealedClass_IsExposed()
        {
            var code = @"public class SampleClass { protected void Member() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "Member");
            Assert.True(method.IsExposed());
            Assert.Contains(type.ExposedMembers(), x => x.Name == "Member");
        }

        [Fact]
        public void ProtectedMethod_OfPublicSealedClass_IsNotExposed()
        {
            var code = @"public sealed class SampleClass { protected void Member() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "Member");
            Assert.False(method.IsExposed());
            Assert.DoesNotContain(type.ExposedMembers(), x => x.Name == "Member");
        }

        [Fact]
        public void ProtectedInternalMethod_OfPublicUnsealedClass_IsExposed()
        {
            var code = @"public class SampleClass { protected internal void Member() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "Member");
            Assert.True(method.IsExposed());
            Assert.Contains(type.ExposedMembers(), x => x.Name == "Member");
        }

        [Fact]
        public void ProtectedInternalMethod_OfPublicSealedClass_IsNotExposed()
        {
            var code = @"public sealed class SampleClass { protected internal void Member() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "Member");
            Assert.False(method.IsExposed());
            Assert.DoesNotContain(type.ExposedMembers(), x => x.Name == "Member");
        }

        [Fact]
        public void PrivateMethod_OfPublicClass_IsNotExposed()
        {
            var code = @"public class SampleClass { private void Member() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "Member");
            Assert.False(method.IsExposed());
            Assert.DoesNotContain(type.ExposedMembers(), x => x.Name == "Member");
        }

        [Fact]
        public void PrivateProtectedMethod_OfPublicClass_IsNotExposed()
        {
            var code = @"public class SampleClass { private protected void Member() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "Member");
            Assert.False(method.IsExposed());
            Assert.DoesNotContain(type.ExposedMembers(), x => x.Name == "Member");
        }

        [Fact]
        public void ExplicitlyImplementedMethod_OfPublicInterface_InPublicClass_IsExposed()
        {
            var code = @"using System; public class SampleClass: IDisposable { void IDisposable.Dispose() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "System.IDisposable.Dispose");
            Assert.True(method.IsExposed());
            Assert.Contains(type.ExposedMembers(), x => x.Name == "System.IDisposable.Dispose");
        }

        [Fact(Skip = "Known bug: https://github.com/StephenClearyApps/DotNetApis/issues/82")]
        public void ExplicitlyImplementedMethod_OfInternalInterface_InPublicClass_IsNotExposed()
        {
            var code = @"internal interface IInternal { void Method(); } public class SampleClass: IInternal { void IInternal.Method() { } }";
            var assembly = Compile(code).Dll;
            var type = assembly.Modules.SelectMany(x => x.Types).Single(x => x.Name == "SampleClass");
            var method = type.Methods.Single(x => x.Name == "IInternal.Method");
            Assert.False(method.IsExposed());
            Assert.DoesNotContain(type.ExposedMembers(), x => x.Name == "IInternal.Method");
        }
    }
}
