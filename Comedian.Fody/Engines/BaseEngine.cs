using System;
using Mono.Cecil;
using System.Linq.Expressions;
using Comedian.Fody.Weavers;

namespace Comedian.Fody.Engines
{
	public abstract class BaseEngine : IEngine
	{
		private readonly ModuleDefinition _module;
		private readonly ILogger _logger;

		protected BaseEngine (ModuleDefinition module, ILogger logger)
		{
			_module = module;
			_logger = logger;
		}

		public TypeReference Get<TType> ()
		{
			return _module.ImportReference (typeof(TType));
		}

		public MethodReference GetMethod<TFunc> (Expression<TFunc> exp)
		{
			
		}

		public abstract IWeaver GetWeaver (MethodDefinition method, FieldDefinition mixin);

		public void Message (string format, params object[] args)
		{
			_logger.Message (format, args);
		}

		public void Warn (string format, params object[] args)
		{
			_logger.Warn (format, args);
		}

		public void Error (string format, params object[] args)
		{
			_logger.Error (format, args);
		}
	}
}

