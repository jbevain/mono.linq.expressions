using System;
using System.Linq.Expressions;

using NUnit.Framework;

namespace Mono.Linq.Expressions {

	[TestFixture]
	public class PredicateBuilderTest : BaseExpressionTest {

		[Test]
		public void AndAlso ()
		{
			var predicate = PredicateBuilder.True<int> ().AndAlso (PredicateBuilder.True<int> ());

			AssertExpression (@"
bool (int)
{
	return true && true;
}
", predicate);
		}

		[Test]
		public void OrElse ()
		{
			var predicate = PredicateBuilder.True<int> ().OrElse (PredicateBuilder.False<int> ());

			AssertExpression (@"
bool (int)
{
	return true || false;
}
", predicate);
		}
	}
}
