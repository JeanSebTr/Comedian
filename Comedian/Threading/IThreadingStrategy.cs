using System;
using System.Collections.Concurrent;

namespace Comedian.Threading
{
	public interface IThreadingStrategy
	{
		bool TryDequeueForThread (ConcurrentQueue<Action> _processingQueue, IThread thread, out Action action);
	}
}

