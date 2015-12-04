using System;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System.Linq;

namespace Comedian.Tests
{
	public static class FluentAssertionsExtensions
	{
		public static void BeAnyOf<TObject>(this ObjectAssertions assertions, params TObject[] expectations)
		{
			Execute.Assertion
				.ForCondition(expectations.Any(exp => assertions.Subject.Equals(exp)))
				.FailWith("Expected {context:object} to be any of {0}{reason}, but found {1}.", expectations, assertions.Subject);
		}
	}
}

