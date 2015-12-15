using System;
using Mono.Cecil;
using Comedian.Fody.Engines;

namespace Comedian.Fody.Weavers
{
	public class AsyncStateMachineWeaver : IWeaver
	{
		private readonly IEngine _engine;
		private readonly TypeDefinition _stateMachineType;
		private readonly FieldDefinition _mixinField;

		public AsyncStateMachineWeaver(IEngine engine, TypeDefinition stateMachineType, FieldDefinition actorMixin)
		{
			_engine = engine;
			_stateMachineType = stateMachineType;
			_mixinField = actorMixin;
		}

		public void Apply ()
		{
			_stateMachineType.Fields.Add (_mixinField);
		}
	}
}

