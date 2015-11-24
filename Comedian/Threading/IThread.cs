using System;

namespace Comedian.Threading
{
	public interface IThread
	{
		void Run (Action action);
	}
}

