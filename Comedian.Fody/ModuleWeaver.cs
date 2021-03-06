﻿using System;
using Mono.Cecil;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Comedian.Fody.Weavers;
using Comedian.Fody.Engines;

namespace Comedian.Fody
{
	#if DEBUG
	public class Comedian
	#else
	public class ModuleWeaver
	#endif
	{
		private static readonly Type ActorAttrType = typeof(ActorAttribute);

		public ModuleDefinition ModuleDefinition { get; set; }
		public Action<string> LogInfo  { get; set; }
		public Action<string> LogWarning  { get; set; }
		public Action<string> LogError { get; set; }

		public void Execute()
		{
			LogComedianVersion ();

			var logger = new Logger (LogInfo, LogWarning, LogError);
			var engine = new ModuleEngine (ModuleDefinition, logger);

			var actorTypes = ModuleDefinition.GetTypes ().Where(HasActorAttribute);
			foreach(var actorType in actorTypes)
			{
				engine.GetWeaver(actorType).Apply ();
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
	}
}

