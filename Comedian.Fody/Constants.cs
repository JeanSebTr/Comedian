using System;
using Mono.Cecil;

namespace Comedian.Fody
{
	public static class Constants
	{
		public const string AsyncStateMachineAttribute = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
		public const FieldAttributes MixinFieldAttr = FieldAttributes.InitOnly | FieldAttributes.Private;
		public const string MixinFieldName = "Comedian<ActorCore>";
	}
}

