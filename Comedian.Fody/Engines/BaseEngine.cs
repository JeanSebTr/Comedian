using System;
using Mono.Cecil;
using System.Linq.Expressions;
using Comedian.Fody.Weavers;

namespace Comedian.Fody.Engines
{
	public abstract class BaseEngine : IEngine
	{
		protected readonly ModuleDefinition _module;
		protected readonly ILogger _logger;

		protected BaseEngine (ModuleDefinition module, ILogger logger)
		{
			_module = module;
			_logger = logger;
		}

		public TypeReference Get(Type type)
		{
			return _module.ImportReference (type);
		}

		public MethodReference Get (MethodReference methodReference)
		{
			return _module.ImportReference (methodReference);
		}

		public TypeReference Get<TType> ()
		{
			return _module.ImportReference (typeof(TType));
		}

		public MethodReference GetMethod<TFunc> (Expression<TFunc> exp)
		{
			if(exp.Body.NodeType == ExpressionType.New)
			{
				var ctor = exp.Body as NewExpression;
				return _module.ImportReference (ctor.Constructor);
			}
			else if(exp.Body.NodeType == ExpressionType.Call)
			{
				var method = exp.Body as MethodCallExpression;
				return _module.ImportReference (method.Method);
			}
			throw new NotImplementedException (string.Format ("Expression.Body.NodeType == {0}", exp.Body.NodeType));
		}

		public abstract IWeaver GetWeaver (MethodDefinition method, FieldDefinition mixin);
		public abstract IWeaver GetWeaver (TypeDefinition type, FieldDefinition mixin = null);

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

