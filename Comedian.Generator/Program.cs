using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Comedian.Generator
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			//ILGenerator gen;

			var assemblyName = new AssemblyName ("Comedian.Generated");
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly (assemblyName, AssemblyBuilderAccess.Save);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule ("Project.ModuleName.Comedian");
			var typeBuilder = moduleBuilder.DefineType ("Project.ModuleName.Comedian.ILoller");
			var methodBuilder = typeBuilder.DefineMethod ("Lol", MethodAttributes.Public | MethodAttributes.Virtual);

			var actorType = typeof(Actor);

			assemblyBuilder.Save ("Comedian.Generated.dll");

			//gen.Emit(OpCodes.Ldtoken
		}

		public static TActor GetActorProxy<TActor>(TActor actor)
		{
			return default(TActor);
		}
	}

	internal class Loller : ILoller
	{
		public void Lol ()
		{
			throw new NotImplementedException ();
		}
	}

	public interface ILoller
	{
		void Lol();
	}
}
