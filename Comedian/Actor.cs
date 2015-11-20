using System;
using Comedian.Queue;
using System.Reflection;
using System.Threading.Tasks;

namespace Comedian
{
	public abstract class Actor
	{
		internal IDispatchQueue _currentQueue = DispatchQueue.Empty;
		internal Int32 _mailboxSize = 0;

		internal readonly object _actor;
		private readonly Scene _scene;

		internal Actor (Scene scene, object actor)
		{
			_scene = scene;
			_actor = actor;
		}

		protected Task<TResult> CallAsync<TResult>(MethodBase method,  params object[] arguments)
		{
			var workItem = new WorkItem<TResult> (this, method, arguments);
			_scene.Dispatch(workItem);
			return workItem.Task;
		}
	}
}

