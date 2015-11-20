using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace Comedian.Queue
{
	public class WorkItem<TResult> : IWorkItem
	{
		private readonly TaskCompletionSource<TResult> _tcs = new TaskCompletionSource<TResult> ();

		private readonly Actor _actor;
		private readonly MethodBase _method;
		private readonly object[] _arguments;

		public WorkItem (Actor actor, MethodBase method, object[] arguments)
		{
			_actor = actor;
			_method = method;
			_arguments = arguments;

			Interlocked.Increment (ref _actor._mailboxSize);
		}

		public Task<TResult> Task
		{
			get { return _tcs.Task; }
		}

		public IDispatchQueue SelectDispatchQueue (IDispatchQueue candidate)
		{
			var oldQueue = Interlocked.CompareExchange<IDispatchQueue> (ref _actor._currentQueue, candidate, DispatchQueue.Empty);
			return oldQueue == DispatchQueue.Empty ? candidate : oldQueue;
		}

		public void Execute()
		{
			var result = _method.Invoke (_actor._actor, _arguments);

			if (Interlocked.Decrement (ref _actor._mailboxSize) == 0)
				Interlocked.Exchange<IDispatchQueue> (ref _actor._currentQueue, DispatchQueue.Empty);
		}
	}

	public interface IWorkItem
	{
		IDispatchQueue SelectDispatchQueue (IDispatchQueue candidate);
		void Execute();
	}
}

