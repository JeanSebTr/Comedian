using System;
using System.Threading;
using System.Threading.Tasks;

namespace Comedian.Sample
{
	[Actor]
	public class SampleActor
	{
		internal readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
		internal Int32 _called = 0;

		public async Task<int> Lol()
		{
			await Task.Delay (200);

			return 11;
		}

//		public void SyncMethod()
//		{
//			Interlocked.Increment (ref _called);
//		}
//
//		public void BlockSyncMethod()
//		{
//			_semaphore.Wait ();
//		}
//
//		internal bool Lol()
//		{
//			return false;
//		}
	}
}

