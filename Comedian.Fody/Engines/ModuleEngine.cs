using System;
using Comedian.Fody.Weavers;
using Mono.Cecil;

namespace Comedian.Fody.Engines
{
	public class ModuleEngine : BaseEngine
	{
		public ModuleEngine(ModuleDefinition module, ILogger logger) : base(module, logger)
		{
		}

		public override IWeaver GetWeaver(TypeDefinition type, FieldDefinition mixin = null)
		{
			var engine = new ActorEngine (type, _logger);
			return new ActorWeaver (engine, type);
		}

		public override IWeaver GetWeaver (MethodDefinition method, FieldDefinition mixin)
		{
			throw new NotImplementedException ();
		}
	}
}

