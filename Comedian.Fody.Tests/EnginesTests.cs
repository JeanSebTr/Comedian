using System;
using NUnit.Framework;
using Comedian.Fody.Engines;
using Mono.Cecil;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Comedian.Fody.Weavers;
using NSubstitute;

namespace Comedian.Fody.Tests
{
	[TestFixture]
	public class EnginesTests
	{
		[Test]
		public void Test()
		{
			var subject = new ModuleEngine (null, null);

			var method = subject.GetMethod<Func<ActorCore, bool>> (a => a.ShouldRunSynchronously ());
			var ctor = subject.GetMethod<Func<ActorCore>> (() => new ActorCore ());
		}

		[Test]
		public void Test2()
		{
			var module = ModuleDefinition.CreateModule ("lollollol", ModuleKind.Dll);


			var type = module.ImportReference (typeof(TestAsync)).Resolve();

			var logger = Substitute.For<ILogger> ();

			var engine = new ActorEngine (type, logger);
			var weaver = new ActorWeaver (engine, type);

			weaver.Apply ();
		}

		private class TestAsync
		{
			[AsyncStateMachine(typeof(StateMachine))]
			public Task<int> Lol()
			{
				return Task.FromResult (1);
			}
		}

		private struct StateMachine : IAsyncStateMachine
		{
			#region IAsyncStateMachine implementation
			public void MoveNext ()
			{
				throw new NotImplementedException ();
			}
			public void SetStateMachine (IAsyncStateMachine stateMachine)
			{
				throw new NotImplementedException ();
			}
			#endregion
			
		}
	}
}

