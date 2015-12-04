using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System.Threading;
using NSubstitute.ExceptionExtensions;

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

			t.Status.Should ().BeAnyOf (TaskStatus.WaitingToRun, TaskStatus.Running);
			subject.Received (0).RealSyncMethod ();

			subject._actorCore.ProcessNext ();

			t.Wait (1000);

			t.Status.Should ().Be (TaskStatus.RanToCompletion);
			subject.Received (1).RealSyncMethod ();
		}

		[Test]
		public void ShouldForwardExceptions()
		{
			var subject = Substitute.For<FakeActor> ();

			subject.RealSyncMethod ().Throws<Exception> ();

			new Action (() => subject.GeneratedSyncMethod ()).ShouldThrow<Exception> ();
		}

		[Test]
		public void ShouldContinueProcessingOnException()
		{
			var subject = Substitute.For<FakeActor> ();

			subject.RealSyncMethod ().Throws<Exception> ();

			subject._actorCore.EnqueueAsync (VoidMethod);

			var t1 = Task.Run (() => subject.GeneratedSyncMethod());
			var t2 = Task.Run (() => subject.GeneratedSyncMethod());

			t1.Status.Should ().BeAnyOf (TaskStatus.WaitingToRun, TaskStatus.Running);
			t2.Status.Should ().BeAnyOf (TaskStatus.WaitingToRun, TaskStatus.Running);

			subject.Received (0).RealSyncMethod ();

			subject._actorCore.ProcessNext ();

			var tasksWithoutExceptions = new []{t1.ContinueWith(t => {}), t2.ContinueWith(t => {})};

			Task.WaitAll (tasksWithoutExceptions, 1000);

			t1.Status.Should ().Be (TaskStatus.Faulted);
			t2.Status.Should ().Be (TaskStatus.Faulted);
		}

		private void VoidMethod()
		{
			
		}
	}
}

