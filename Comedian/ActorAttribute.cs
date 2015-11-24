using System;

namespace Comedian
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ActorAttribute : Attribute
	{
		public ActorAttribute ()
		{
		}
	}
}

