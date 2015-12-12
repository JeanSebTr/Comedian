using System;
using Mono.Cecil;
using System.Linq.Expressions;
using Comedian.Fody.Weavers;

namespace Comedian.Fody
{
	public interface IEngine : ILogger
	{
		TypeReference Get<TType>();

		MethodReference GetMethod<TFunc>(Expression<TFunc> exp);

		IWeaver GetWeaver (MethodDefinition method, FieldDefinition mixin);
	}
}

