using System;
using System.Collections.Concurrent;

namespace Comedian.Queue
{
	public class DispatchQueue : IDispatchQueue
	{
		public static readonly IDispatchQueue Empty = new EmptyQueue();

		private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>(); 

		public DispatchQueue ()
		{
		}

		public void Dispatch (IWorkItem workItem)
		{
			throw new NotImplementedException ();
		}

		class EmptyQueue : IDispatchQueue
		{
			public void Dispatch (IWorkItem workItem)
			{
				throw new NotImplementedException ();
			}
		}
	}
}

