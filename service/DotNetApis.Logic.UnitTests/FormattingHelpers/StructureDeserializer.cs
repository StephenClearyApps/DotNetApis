using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetApis.Structure.Entities;
using DotNetApis.Structure.GenericConstraints;
using DotNetApis.Structure.Literals;
using DotNetApis.Structure.Locations;
using DotNetApis.Structure.TypeReferences;
using DotNetApis.Structure.Xmldoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetApis.Logic.UnitTests.FormattingHelpers
{
    public static class StructureDeserializer
    {
		public static readonly JsonSerializer Instance = new JsonSerializer
		{
			Converters =
			{
				new EntityConverter(),
				new GenericConstraintConverter(),
				new LiteralConverter(),
				new LocationConverter(),
				new TypeReferenceConverter(),
				new XmldocConverter(),
			}
		};

	    private abstract class ConverterBase<T> : JsonConverter
	    {
		    public override bool CanWrite => false;

		    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

		    public override bool CanConvert(Type objectType) => objectType == typeof(T);
	    }

	    private abstract class KindConverterBase<T> : ConverterBase<T>
	    {
		    protected abstract Type KindType(int kind);

		    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		    {
			    var jobject = JObject.Load(reader);
			    var kind = jobject["k"]?.Value<int>() ?? 0;
			    var type = KindType(kind);
			    return type == null ? null : jobject.ToObject(type, serializer);
		    }
	    }

	    private sealed class EntityConverter: KindConverterBase<IEntity>
		{
			protected override Type KindType(int kind)
			{
				switch (kind)
				{
					case 0:
					case 1:
					case 2:
						return typeof(TypeEntity);
					case 3: return typeof(EnumEntity);
					case 4: return typeof(DelegateEntity);
					case 5: return typeof(MethodEntity);
					case 6: return typeof(PropertyEntity);
					case 7: return typeof(EventEntity);
					case 8: return typeof(FieldEntity);
					default: return null;
				}
			}
		}

	    private sealed class GenericConstraintConverter : KindConverterBase<IGenericConstraint>
	    {
		    protected override Type KindType(int kind)
		    {
			    switch (kind)
			    {
				    case 0: return typeof(ClassGenericConstraint);
				    case 1: return typeof(StructGenericConstraint);
				    case 2: return typeof(NewGenericConstraint);
				    case 3: return typeof(TypeGenericConstraint);
				    default: return null;
			    }
		    }
	    }

	    private sealed class LiteralConverter : KindConverterBase<ILiteral>
	    {
		    protected override Type KindType(int kind)
		    {
			    switch (kind)
			    {
				    case 0: return typeof(NullLiteral);
				    case 1: return typeof(PrimitiveLiteral);
				    case 2: return typeof(ArrayLiteral);
				    case 3: return typeof(TypeofLiteral);
				    case 4: return typeof(EnumLiteral);
				    default: return null;
			    }
		    }
	    }

	    private sealed class LocationConverter : ConverterBase<ILocation>
	    {
		    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		    {
			    if (reader.Value is string stringValue)
				    return new CurrentPackageLocation { DnaId = stringValue, };
			    var jobject = JObject.Load(reader);
			    if (jobject["p"] == null)
				    return jobject.ToObject<ReferenceLocation>(serializer);
			    return jobject.ToObject<DependencyLocation>(serializer);
		    }
		}

		private sealed class TypeReferenceConverter: KindConverterBase<ITypeReference>
		{
			protected override Type KindType(int kind)
			{
				switch (kind)
				{
					case 0: return typeof(TypeTypeReference);
					case 1: return typeof(KeywordTypeReference);
					case 2: return typeof(GenericInstanceTypeReference);
					case 3: return typeof(DynamicTypeReference);
					case 4: return typeof(GenericParameterTypeReference);
					case 5: return typeof(ArrayTypeReference);
					case 6: return typeof(ReqmodTypeReference);
					case 7: return typeof(PointerTypeReference);
					default: return null;
				}
			}
		}

	    private sealed class XmldocConverter : ConverterBase<IXmldocNode>
	    {
		    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		    {
			    if (reader.Value is string stringValue)
				    return new StringXmldocNode { Text = stringValue, };
			    return serializer.Deserialize<XmlXmldocNode>(reader);
		    }
	    }
	}
}
