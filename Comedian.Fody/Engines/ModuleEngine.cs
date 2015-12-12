using System;
using Comedian.Fody.Weavers;
using Mono.Cecil;

namespace Comedian.Fody.Engines
{
	public class ModuleEngine : BaseEngine
	{
		public IWeaver GetWeaver (MethodDefinition method, FieldDefinition mixin)
		{
			throw new NotImplementedException ();
		}
	}
}

