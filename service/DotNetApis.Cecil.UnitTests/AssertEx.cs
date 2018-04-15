using DotNetApis.Cecil;
using DotNetApis.Common;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

internal static class AssertEx
{
    public static void AssertEqual(this FriendlyName @this, string expectedSimpleName, string expectedQualifiedName, string expectedFullyQualifiedName)
    {
        Assert.Equal(expectedSimpleName, @this.SimpleName);
        Assert.Equal(expectedQualifiedName, @this.QualifiedName);
        Assert.Equal(expectedFullyQualifiedName, @this.FullyQualifiedName);
    }
}
