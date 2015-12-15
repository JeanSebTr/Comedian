using System;
using Mono.Cecil;
using System.Linq.Expressions;
using Comedian.Fody.Weavers;

namespace Comedian.Fody.Engines
{
	public interface IEngine : ILogger
	{
		TypeReference Get (Type type);
		MethodReference Get (MethodReference methodReference);

		TypeReference Get<TType>();

		MethodReference GetMethod<TFunc>(Expression<TFunc> exp);

		IWeaver GetWeaver (TypeDefinition type, FieldDefinition mixin = null);
		IWeaver GetWeaver (MethodDefinition method, FieldDefinition mixin);
	}
}

