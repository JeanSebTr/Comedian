using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Linq;
using Comedian.Fody.Engines;

namespace Comedian.Fody.Weavers
{
	public class AsyncMethodWeaver : IWeaver
	{
		private readonly IEngine _engine;
		private readonly MethodDefinition _method;
		private readonly FieldDefinition _actorMixin;
		private readonly FieldDefinition _stateMachineMixin;

		public AsyncMethodWeaver (IEngine engine, MethodDefinition method, FieldDefinition actorMixin, FieldDefinition stateMachineMixin)
		{
			_engine = engine;
			_method = method;
			_actorMixin = actorMixin;
			_stateMachineMixin = stateMachineMixin;
		}
		//			IL_0000: ldloca.s 0
		//			IL_0002: ldarg.1
		//			IL_0003: stfld int32 Comedian.ActorCore/'<Lol>c__async0'::i
		//			IL_0008: ldloca.s 0
		//			IL_000a: call valuetype [System.Threading.Tasks]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [System.Threading.Tasks]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::Create()
		//			IL_000f: stfld valuetype [System.Threading.Tasks]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32> Comedian.ActorCore/'<Lol>c__async0'::$builder
		// TODO : check if ShouldRunSynchronously
		// ldarg.0
		// ldfld actorMixin
		// call instance ... ShouldRunSynchronously
		// if true goto IL_0014
		// TODO : queue
		// ldarg.0
		// ldloca.s 0
		// ldflda valuetype [System.Threading.Tasks]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32> Comedian.ActorCore/'<Lol>c__async0'::$builder
		// ldloca.s 0
		// call instance … Enqueue …
		// ret
		//			IL_0014: ldloca.s 0
		//			IL_0016: ldflda valuetype [System.Threading.Tasks]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32> Comedian.ActorCore/'<Lol>c__async0'::$builder

		//			IL_001b: dup

		//			IL_001c: ldloca.s 0
		//			IL_001e: call instance void valuetype [System.Threading.Tasks]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::Start<valuetype Comedian.ActorCore/'<Lol>c__async0'>(!!0&)
		//			IL_0023: call instance class [System.Threading.Tasks]System.Threading.Tasks.Task`1<!0> valuetype [System.Threading.Tasks]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<int32>::get_Task()
		//			IL_0028: ret
		public void Apply()
		{
			var endOfStateMachineInit = GetEndOfStateMachineInitialization (_method.Body);
			var stateMachineStarting = endOfStateMachineInit.Next;

			var ilp = _method.Body.GetILProcessor ();
			ilp.Body.SimplifyMacros ();

			var last = AddSetActorMixin (ilp, endOfStateMachineInit);
			last = AddConditionGoto (ilp, last, stateMachineStarting);
			AddEnqueueOps (ilp, last);

			ilp.Body.OptimizeMacros ();
		}

		/// <summary>
		/// stateMachine.mixin = this.mixin;
		/// </summary>
		/// <returns>The last instruction added</returns>
		/// <param name="ilp">Il processor</param>
		/// <param name="after">Add instructions after this one</param>
		private Instruction AddSetActorMixin(ILProcessor ilp, Instruction after)
		{
			var loadStateMachineVar = ilp.Create (OpCodes.Ldloca_S, ilp.Body.Variables[0]);
			var loadThis = ilp.Create (OpCodes.Ldarg_0);
			var loadField = ilp.Create (OpCodes.Ldfld, _actorMixin);
			var storeField = ilp.Create (OpCodes.Stfld, _stateMachineMixin);

			ilp.InsertAfter (after, loadStateMachineVar);
			ilp.InsertAfter (loadStateMachineVar, loadThis);
			ilp.InsertAfter (loadThis, loadField);
			ilp.InsertAfter (loadField, storeField);

			return storeField;
		}

		private Instruction AddConditionGoto (ILProcessor ilp, Instruction last, Instruction stateMachineStarting)
		{
			var shouldRunSynchronouslyMethod = _engine.GetMethod<Func<ActorCore, bool>> (a => a.ShouldRunSynchronously ());

			var loadThis = ilp.Create (OpCodes.Ldarg_0);
			var loadField = ilp.Create (OpCodes.Ldfld, _actorMixin);
			var callMethod = ilp.Create (OpCodes.Call, shouldRunSynchronouslyMethod);
			var gotoNext = ilp.Create (OpCodes.Brtrue_S, stateMachineStarting);

			ilp.InsertAfter (last, loadThis);
			ilp.InsertAfter (loadThis, loadField);
			ilp.InsertAfter (loadField, callMethod);
			ilp.InsertAfter (callMethod, gotoNext);

			return gotoNext;
		}

		private Instruction AddEnqueueOps (ILProcessor ilp, Instruction last)
		{
			var enqueueMethod = GetEnqueueMethodReference();
			var endInstruction = ilp.Body.Instructions.Last ();

			var loadThis = ilp.Create (OpCodes.Ldarg_0);
			var loadMixinField = ilp.Create (OpCodes.Ldfld, _actorMixin);
			var loadStateMachineVar_1 = ilp.Create (OpCodes.Ldloca_S, ilp.Body.Variables[0]);
			var loadBuilderField = ilp.Create (OpCodes.Ldfld, GetBuilderField ());
			var loadStateMachineVar_2 = ilp.Create (OpCodes.Ldloca_S, ilp.Body.Variables[0]);
			var callMethod = ilp.Create (OpCodes.Call, enqueueMethod);
			var gotoEnd = ilp.Create (OpCodes.Br, endInstruction);

			ilp.InsertAfter (last, loadThis);
			ilp.InsertAfter (loadThis, loadMixinField);
			ilp.InsertAfter (loadMixinField, loadStateMachineVar_1);
			ilp.InsertAfter (loadStateMachineVar_1, loadBuilderField);
			ilp.InsertAfter (loadBuilderField, loadStateMachineVar_2);
			ilp.InsertAfter (loadStateMachineVar_2, callMethod);
			ilp.InsertAfter (callMethod, gotoEnd);

			return gotoEnd;
		}

		private MethodReference GetEnqueueMethodReference()
		{
			var actorMixinType = _engine.Get<ActorCore> ().Resolve();

			MethodReference enqueueMethod = actorMixinType.Methods.Single (IsCorrespondingEnqueueMethod);
			enqueueMethod = _engine.Get (enqueueMethod);

			var methodInstance = new GenericInstanceMethod (enqueueMethod);

			if(_method.ReturnType.IsGenericInstance)
			{
				var taskReturnArg = (_method.ReturnType as GenericInstanceType).GenericArguments.First ();
				methodInstance.GenericArguments.Add (taskReturnArg);
			}

			var stateMachineType = GetStateMachineType ();
			methodInstance.GenericArguments.Add (stateMachineType);

			return methodInstance;
		}

		private bool IsCorrespondingEnqueueMethod(MethodDefinition method)
		{
			if (method.Name != "Enqueue")
				return false;

			if (_method.ReturnType.IsGenericInstance && method.GenericParameters.Count == 2)
				return true;

			return _method.ReturnType.FullName == method.ReturnType.FullName;
		}

		private FieldReference GetBuilderField()
		{
			var stateMachineType = GetStateMachineType().Resolve();

			return stateMachineType.Fields.Single (f => f.FieldType.Namespace == "System.Runtime.CompilerServices" && f.FieldType.Name.StartsWith("Async", StringComparison.Ordinal));
		}

		private TypeReference GetStateMachineType()
		{
			var stateMachineAttr = _method.CustomAttributes.Single (attr => attr.AttributeType.FullName == Constants.AsyncStateMachineAttribute);
			return stateMachineAttr.ConstructorArguments.First ().Value as TypeReference;
		}

		private Instruction GetEndOfStateMachineInitialization(MethodBody body)
		{
			var instructions = body.Instructions;
			for(int i=instructions.Count - 1; i>=0; i--)
			{
				if (instructions [i].OpCode == OpCodes.Stfld)
					return instructions [i];
			}
			_engine.Error ("Method ");
			throw new InvalidProgramException ("");
		}
	}
}

