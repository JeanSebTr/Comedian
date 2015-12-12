using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Comedian.Fody.Weavers
{
	public class AsyncMethodWeaver
	{
		private readonly IEngine _engine;
		private readonly MethodDefinition _method;

		public AsyncMethodWeaver (IEngine engine, MethodDefinition method)
		{
			_engine = engine;
			_method = method;
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


			var ilp = _method.Body.GetILProcessor ();
			//ilp.
		}

		private Instruction GetEndOfStateMachineInitialization(MethodBody body)
		{
			var instructions = body.Instructions;
			for(int i=instructions.Count; i>=0; i--)
			{
				if (instructions [i].OpCode == OpCodes.Stfld)
					return instructions [i];
			}
			_engine.Error ("Method ");
			throw new InvalidProgramException ("");
		}
	}
}

