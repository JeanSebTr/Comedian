using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using Comedian.Queue;
using System.Threading;

namespace Comedian
{
	public abstract class Actor
	{
		private readonly ConcurrentQueue<IWorkItem> _mailbox = new ConcurrentQueue<IWorkItem> ();
		private Int32 _mailboxSize = 0;

		internal readonly object _actor;
		private readonly Scene _scene;

		internal Actor (Scene scene, object actor)
		{
			_scene = scene;
			_actor = actor;

//			var sc = new SynchronizationContext ();
//
//			sc.P
		}

		protected Task<TResult> CallAsync<TResult>(MethodBase method,  params object[] arguments)
		{
			var workItem = new WorkItem<TResult> (this, method, arguments);

			_mailbox.Enqueue (workItem);
			StartProcessingIfFirstInMailbox ();
			
			return workItem.Task;
		}

		internal void ProcessMailbox()
		{
			IWorkItem workItem;
			do
			{
				if(!_mailbox.TryDequeue(out workItem))
					throw new InvalidOperationException("Actor has an empty Mailbox");


			}
			while(ShouldContinueProcessing());
		}

		private void StartProcessingIfFirstInMailbox()
		{
			if(Interlocked.Increment(ref _mailboxSize) == 1)
				_scene.Process (this);
		}

		private bool ShouldContinueProcessing()
		{
			return Interlocked.Decrement (ref _mailboxSize) > 0;
		}
	}
}

