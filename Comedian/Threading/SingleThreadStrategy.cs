using System;

namespace Comedian.Threading
{
	public class SingleThreadStrategy : IThreadingStrategy
	{
		public SingleThreadStrategy ()
		{
		}

		public bool TryDequeueForThread (System.Collections.Concurrent.ConcurrentQueue<Action> _processingQueue, IThread thread, out Action action)
		{
			action = null;
			return false;
		}
	}
}

