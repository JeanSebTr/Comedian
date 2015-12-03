using System;

namespace Comedian.Tests.ActorCore
{
	[Actor]
	public class FakeActor
	{
		internal ActorCore<FakeActor> _actorCore = new ActorCore<FakeActor>();

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

