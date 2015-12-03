using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System.Threading;

namespace Comedian.Tests.ActorCore
{
	[TestFixture]
	public class SyncCall
	{
		[Test]
		public void ShouldBeRunImmediatelyWhenMailboxIsEmpty()
		{
			var subject = Substitute.For<FakeActor> ();

			subject.GeneratedSyncMethod ();

			subject.Received (1).RealSyncMethod ();
		}

		[Test]
		public void ShouldRunLaterWhenMailboxIsNotEmpty()
		{
			var subject = Substitute.For<FakeActor> ();

			subject._actorCore.EnqueueAsync (VoidMethod);

			var t = Task.Run (() => subject.GeneratedSyncMethod());
			Thread.Sleep (10);

			t.Status.Should ().Be (TaskStatus.Running);
			subject.Received (0).RealSyncMethod ();

			subject._actorCore.ProcessNext ();
			Thread.Sleep (10);

			t.Status.Should ().Be (TaskStatus.RanToCompletion);
			subject.Received (1).RealSyncMethod ();
		}

		private void VoidMethod()
		{
			
		}
	}
}

