using FluentAssertions;
using NUnit.Framework;
using System.Reflection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Comedian.Tests
{
	[TestFixture]
	public class DotNetFeaturesTests
	{
		[Test]
		public void ShouldInvokeClassMethodWhenInvokingMethodBaseOfInterface ()
		{
			var classType = new ClassType ();

			classType.Called.Should ().BeFalse ();

			var methodBase = typeof(IInterfaceType).GetMethod("Method");
			methodBase.Invoke (classType, new object[0]);

			classType.Called.Should ().BeTrue ();
		}

		[Test]
		public void ShouldReturnSameMethodBase()
		{
			var type = typeof(IInterfaceType);
			var methodBase = type.GetMethod("Method");

			var methodFromHandle = MethodBase.GetMethodFromHandle (methodBase.MethodHandle);
			var methodFromHandleWithType = MethodBase.GetMethodFromHandle (methodBase.MethodHandle, type.TypeHandle);

			methodFromHandle.Should ().Be (methodBase);
			methodFromHandleWithType.Should ().Be (methodBase);
		}

		[Test]
		public void ShouldReturnOneWhenIncrementingFromZero()
		{
			Int32 num = 0;
			Interlocked.Increment (ref num).Should().Be(1);
			Interlocked.Decrement (ref num).Should ().Be (0);
		}

		private interface IInterfaceType
		{
			void Method();
		}

		private class ClassType : IInterfaceType
		{
			public bool Called = false;

			public void Method ()
			{
				Called = true;
			}
			
		}

	}
}

