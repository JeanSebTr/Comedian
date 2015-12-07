using System;

namespace Comedian.Tests.ActorCore
{
	[Actor]
	public class FakeActor
	{
		internal Comedian.ActorCore _actorCore = new Comedian.ActorCore();

		public FakeActor ()
		{
		}

		public int GeneratedSyncMethod()
		{
			var method = new Func<int> (RealSyncMethod);
			return _actorCore.EnqueueSync<int> (this, method.Method);
		}

		public virtual int RealSyncMethod()
		{
			return 42;
		}
	}
}

