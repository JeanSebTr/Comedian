using System;
using System.Threading;

namespace Comedian.Sample
{
	[Actor]
	public class SampleActor
	{
		internal readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
		internal Int32 _called = 0;

		public void SyncMethod()
		{
			Interlocked.Increment (ref _called);
		}

		public void BlockSyncMethod()
		{
			_semaphore.Wait ();
		}
	}
}

