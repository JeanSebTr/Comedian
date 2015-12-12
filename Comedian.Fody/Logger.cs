using System;

namespace Comedian.Fody
{
	public class Logger : ILogger
	{
		private readonly Action<string> _message;
		private readonly Action<string> _warning;
		private readonly Action<string> _error;

		public Logger (Action<string> message, Action<string> warning, Action<string> error)
		{
			_message = message;
			_warning = warning;
			_error = error;
		}

		public void Message (string format, params object[] args)
		{
			_message (string.Format (format, args));
		}

		public void Warn (string format, params object[] args)
		{
			_warning (string.Format (format, args));
		}

		public void Error (string format, params object[] args)
		{
			_error (string.Format (format, args));
		}
	}

	public interface ILogger
	{
		void Message(string format, params object[] args);
		void Warn(string format, params object[] args);
		void Error(string format, params object[] args);
	}
}

