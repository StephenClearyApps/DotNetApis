using DotNetApis.Cecil;
using DotNetApis.Common;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

internal static class AssertEx
{
    public static void AssertEqual(this FriendlyName @this, string expectedSimpleName, string expectedQualifiedName, string expectedFullyQualifiedName)
    {
        Assert.Equal(expectedSimpleName, @this.SimpleName);
        Assert.Equal(expectedQualifiedName, @this.QualifiedName);
        Assert.Equal(expectedFullyQualifiedName, @this.FullyQualifiedName);
    }

    public static void AssertXmldoc(this XDocument @this, string expectedValue, IMemberDefinition member, string elementName = "summary")
    {
        var doc = @this.Descendants("member").FirstOrDefault(x => x.Attribute("name")?.Value == member.MemberXmldocIdentifier()).Element(elementName);
        Assert.Equal(expectedValue, string.Join("", doc.Nodes().Select(x => x.ToString(SaveOptions.DisableFormatting))));
    }
}
