using System;

namespace Comedian.Threading
{
	public interface IThreadManager
	{
		IThread Create();
		void Release (IThread thread);
	}
}

