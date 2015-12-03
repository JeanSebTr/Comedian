using System;
using Mono.Cecil;
using System.Linq;
using System.Reflection;

namespace Comedian.Fody
{
	public class Comedian : BaseWeaver
	{
		public override ModuleDefinition ModuleDefinition { get; set; }
		public override Action<string> LogWarning  { get; set; }

		public override void Execute()
		{
			WeaveModule (ModuleDefinition);
		}
	}


	public class ModuleWeaver : BaseWeaver
	{
		public override ModuleDefinition ModuleDefinition { get; set; }
		public override Action<string> LogWarning  { get; set; }

		public override void Execute()
		{
			WeaveModule (ModuleDefinition);
		}
	}

	public abstract class BaseWeaver
	{
		private static readonly Type ActorAttrType = typeof(ActorAttribute);

		public abstract ModuleDefinition ModuleDefinition { get; set; }
		public abstract Action<string> LogWarning  { get; set; }
		public abstract void Execute();

		protected void WeaveModule(ModuleDefinition module)
		{
			LogComedianVersion ();

			var actorTypes = module.Types.Where(HasActorAttribute);
			foreach(var actorType in actorTypes)
			{
				TransformType (actorType);
			}
		}

		private void LogComedianVersion()
		{
			var version = Assembly.GetExecutingAssembly ().GetName().Version.ToString();
			LogWarning ("Comedian.Fody v" + version);
		}

		private bool HasActorAttribute(TypeDefinition typeDefinition)
		{
			return typeDefinition.CustomAttributes.Any (attr => attr.AttributeType.FullName == ActorAttrType.FullName);
		}

		private void TransformType(TypeDefinition actorType)
		{
			LogWarning (actorType.FullName + " 7 lol");
			
		}
	}
}

