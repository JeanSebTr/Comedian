using System;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Comedian.Fody.Engines;

namespace Comedian.Fody.Weavers
{
	public class ActorWeaver : IWeaver
	{
		private readonly IEngine _engine;
		private readonly TypeDefinition _weavedType;

		public ActorWeaver (IEngine engine, TypeDefinition typeDefinition)
		{
			_engine = engine;
			_weavedType = typeDefinition;
		}

		public void Apply()
		{
			CreateMixinField ();

			WeaveMethods ();
		}

		private FieldDefinition _actorMixinField;
		private void CreateMixinField()
		{
			var actorType = _engine.Get<ActorCore> ();
			_actorMixinField = new FieldDefinition (Constants.MixinFieldName, Constants.MixinFieldAttr, actorType);
			_weavedType.Fields.Add (_actorMixinField);

			var actorCtor = _weavedType.Methods.Single (m => m.IsConstructor);
			var mixinCtor = _engine.GetMethod<Func<ActorCore>> (() => new ActorCore ());

			var ilp = actorCtor.Body.GetILProcessor ();

			var loadThis = ilp.Create (OpCodes.Ldarg_0);
			var callCtor = ilp.Create (OpCodes.Newobj, mixinCtor);
			var saveMixin = ilp.Create (OpCodes.Stfld, _actorMixinField);

			if(actorCtor.Body.Instructions.Count > 0)
			{
				ilp.InsertBefore (actorCtor.Body.Instructions [0], loadThis);
			}
			else
			{
				ilp.Append (loadThis);
			}
			ilp.InsertAfter (loadThis, callCtor);
			ilp.InsertAfter (callCtor, saveMixin);
		}

		private void WeaveMethods()
		{
			var methodsToWeave = new List<MethodDefinition> ();
			foreach(var method in _weavedType.Methods)
			{
				if (method.IsConstructor)
					continue;

				if(method.IsStatic)
				{
					_engine.Warn ("Static method {0}.{1} won't be made thread safe, static methods aren't supported.",
						method.DeclaringType.Name, method.Name);
					continue;
				}

				if ((method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private)
					continue;

				_engine.Warn ("Found {2} {0}.{1}", _weavedType.Name, method.Name, method.Attributes & MethodAttributes.MemberAccessMask);

				methodsToWeave.Add (method);
			}
			foreach(var method in methodsToWeave)
			{
				_engine.GetWeaver (method, _actorMixinField).Apply ();
			}
		}
	}
}

