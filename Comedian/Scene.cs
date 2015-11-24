using System.Collections.Concurrent;
using System.Threading;
using Comedian.Queue;
using System;
using Comedian.Threading;

namespace Comedian
{
	public class Scene : SynchronizationContext
	{
		private readonly IThreadingStrategy _strategy;

		//private delegate void ProcessWorkItem()

		private ConcurrentQueue<Action> _processingQueue = new ConcurrentQueue<Action>();
		private Int32 _processingQueueSize = 0;

		public Scene (IThreadingStrategy strategy)
		{
			_strategy = strategy;
		}

		internal void Process(Actor actor)
		{
			//base.Post ();
		}

		public override void Post (SendOrPostCallback callback, object state)
		{
			_processingQueue.Enqueue (() => callback (state));
		}

		public override void Send (SendOrPostCallback callback, object state)
		{
			using(var semaphore = new SemaphoreSlim(0, 1))
			{
				Exception forwardedException = null;
				Post(s => {
					try{
						callback(s);
					}
					catch(Exception e)
					{
						forwardedException = e;
					}
					finally{
						semaphore.Release(1);
					}
				}, state);

				semaphore.Wait ();

				if (forwardedException != null)
					throw forwardedException;
			}
		}

		private void RunProcessingThread(IThread thread)
		{
			Action action;
			while(_strategy.TryDequeueForThread(_processingQueue, thread, out action))
			{
				//try{ action(); }

			}

		}
	}
}

