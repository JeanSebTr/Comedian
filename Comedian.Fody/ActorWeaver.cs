using System;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;

namespace Comedian.Fody
{
	public class ActorWeaver
	{
		private const string AsyncStateMachineAttribute = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
		private static readonly Type ActorMixinType = typeof(ActorCore);

		private readonly ModuleDefinition _module;
		private readonly TypeDefinition _weavedType;

		public ActorWeaver (ModuleDefinition moduleDefinition, TypeDefinition typeDefinition)
		{
			_module = moduleDefinition;
			_weavedType = typeDefinition;
		}

		public void Apply()
		{
			ImportReferences ();
			CreateMixinField ();

			WeaveMethods ();
		}

		private TypeReference _actorMixinType;
		private MethodReference _actorMixinCtor;

		private void ImportReferences()
		{
			_actorMixinType = _module.ImportReference (ActorMixinType);
			_actorMixinCtor = _module.ImportReference (_actorMixinType.Resolve ().Methods.Single (m => m.IsConstructor && !m.IsStatic));
		}

		private FieldDefinition _actorMixinField;
		private void CreateMixinField()
		{
			const FieldAttributes mixinFieldAttr = FieldAttributes.InitOnly | FieldAttributes.Private;
			_actorMixinField = new FieldDefinition ("Comedian<ActorCore>", mixinFieldAttr, _actorMixinType);
			_weavedType.Fields.Add (_actorMixinField);

			var actorCtor = _weavedType.Methods.Single (m => m.IsConstructor);

			var newActoCoreInstruct = Instruction.Create (OpCodes.Newobj, _actorMixinCtor);
			var saveActorFieldInstruct = Instruction.Create (OpCodes.Stfld, _actorMixinField);

			actorCtor.Body.Instructions.Insert (0, Instruction.Create (OpCodes.Ldarg_0));
			actorCtor.Body.Instructions.Insert (1, newActoCoreInstruct);
			actorCtor.Body.Instructions.Insert (2, saveActorFieldInstruct);
		}

		private void WeaveMethods()
		{
			foreach(var method in _weavedType.Methods)
			{
				

				if ((method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private)
					continue;

				if (method.CustomAttributes.Any (IsAsyncStateMachineAttribute))
					WeaveAsyncMethod (method);
			}
		}

		private void WeaveAsyncMethod(MethodDefinition method)
		{
			
		}

		private bool IsAsyncStateMachineAttribute(CustomAttribute attr)
		{
			return attr.AttributeType.FullName == AsyncStateMachineAttribute;
		}
	}
}

