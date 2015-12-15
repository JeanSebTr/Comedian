using System;
using Mono.Cecil;
using Comedian.Fody.Weavers;
using System.Linq;

namespace Comedian.Fody.Engines
{
	public class ActorEngine : BaseEngine
	{
		public ActorEngine (TypeDefinition type, ILogger logger) : base(type.Module, logger)
		{
		}

		public override IWeaver GetWeaver (MethodDefinition method, FieldDefinition mixin)
		{
			if (method.CustomAttributes.Any (a => a.AttributeType.FullName == Constants.AsyncStateMachineAttribute))
				return GetAsyncMethodWeaver (method, mixin);
			return new FakeWeaver ();
			throw new NotImplementedException ();
		}

		public override IWeaver GetWeaver (TypeDefinition type, FieldDefinition mixin = null)
		{
			throw new NotImplementedException ();
		}

		private IWeaver GetAsyncMethodWeaver(MethodDefinition method, FieldDefinition actorMixin)
		{
			var stateMachineMixin = new FieldDefinition (Constants.MixinFieldName, Constants.MixinFieldAttr, actorMixin.FieldType);

			var stateMachineAttr = method.CustomAttributes.Single (a => a.AttributeType.FullName == Constants.AsyncStateMachineAttribute);
			var ctorArg = stateMachineAttr.ConstructorArguments.First ().Value as TypeReference;

			var stateMachineWeaver = new AsyncStateMachineWeaver (this, ctorArg.Resolve(), stateMachineMixin);
			var methodWeaver = new AsyncMethodWeaver (this, method, actorMixin, stateMachineMixin);

			return new DualWeaver(stateMachineWeaver, methodWeaver);
		}

		private class FakeWeaver : IWeaver
		{
			public void Apply()
			{
				
			}
		}

		private class DualWeaver : IWeaver
		{
			private readonly IWeaver _first;
			private readonly IWeaver _second;

			public DualWeaver(IWeaver first, IWeaver second)
			{
				_first = first;
				_second = second;
			}

			public void Apply()
			{
				_first.Apply ();
				_second.Apply ();
			}
		}
	}
}

