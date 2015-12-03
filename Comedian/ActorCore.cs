using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Comedian
{
	internal class ActorCore
	{
		[ThreadStatic]
		protected static ActorCore _currentlyExecuting = null;
	}

	internal class ActorCore<TActor> : ActorCore
	{
		private readonly ConcurrentQueue<Action> _mailbox = new ConcurrentQueue<Action> ();
		private Int32 _mailboxSize = 0;

		public async Task<int> Lol()
		{
			await Task.Delay (1000);

			return 42;
		}

		public void Enqueue<TStateMachine>(AsyncVoidMethodBuilder builder, TStateMachine stateMachine)
			where TStateMachine : IAsyncStateMachine
		{
			EnqueueAsync (() => builder.Start (ref stateMachine));
		}

		public void Enqueue<TStateMachine>(AsyncTaskMethodBuilder builder, TStateMachine stateMachine)
			where TStateMachine : IAsyncStateMachine
		{
			EnqueueAsync (() => builder.Start (ref stateMachine));
		}

		public void Enqueue<TResult, TStateMachine>(AsyncTaskMethodBuilder<TResult> builder, TStateMachine stateMachine)
			where TStateMachine : IAsyncStateMachine
		{
			EnqueueAsync (() => builder.Start (ref stateMachine));
		}

		internal void EnqueueAsync(Action action)
		{
			if (ShouldRunSynchronously ())
				action ();
			else
				_mailbox.Enqueue (action);
		}

		public TResult EnqueueSync<TResult>(object self, MethodBase method, params object[] arguments)
		{
			TResult result = default(TResult);
			if(ShouldRunSynchronously())
			{
				try{
					result = (TResult)method.Invoke (self, arguments);
				}
				finally{
					ProcessNext ();
				}
			}
			else
			{
				using (var semaphore = new SemaphoreSlim (0, 1))
				{
					Exception exception = null;
					_mailbox.Enqueue (() => {
						try {
							result = (TResult)method.Invoke (self, arguments);
						}
						catch(Exception e)
						{ exception = e; }
						finally {
							semaphore.Release();
							ProcessNext ();
						}
					});
					semaphore.Wait ();
					if (exception != null)
						throw exception;
				}
			}
			return result;
		}

		public void ProcessNext()
		{
			Action action;
			if (Volatile.Read(ref _mailboxSize) == 0 || !_mailbox.TryDequeue (out action))
				return;

			Interlocked.Decrement (ref _mailboxSize);
			action ();
		}

		private bool ShouldRunSynchronously()
		{
			const Int32 FIRST_ITEM_ADDED = 1;
			return Interlocked.Increment (ref _mailboxSize) == FIRST_ITEM_ADDED;
		}

		private bool IsCallingSelf()
		{
			return Object.ReferenceEquals (_currentlyExecuting, this);
		}
	}
}

