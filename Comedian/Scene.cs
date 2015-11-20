using System;
using System.Threading.Tasks;
using System.Reflection;
using Comedian.Queue;
using System.Threading;

namespace Comedian
{
	public class Scene
	{
		private readonly IDispatchQueue _queue;

		public Scene ()
		{
			_queue = new DispatchQueue ();
		}

		internal void Dispatch(IWorkItem workItem)
		{
			var queue = GetNextQueue ();

			queue = workItem.SelectDispatchQueue (queue);
			queue.Dispatch (workItem);
		}

		private IDispatchQueue GetNextQueue()
		{
			return _queue;
		}
	}
}

