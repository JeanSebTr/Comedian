using System;
using System.Reflection;

namespace Comedian.Queue
{
	public interface IDispatchQueue
	{
		void Dispatch (IWorkItem workItem);
	}


}

