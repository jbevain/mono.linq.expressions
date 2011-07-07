//
// PredicateBuilderTest.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2010 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Linq.Expressions;

using NUnit.Framework;

namespace Mono.Linq.Expressions {

	[TestFixture]
	public class PredicateBuilderTest : BaseExpressionTest {

		class User {
			public bool IsFoo { get; set; }
			public bool IsBar { get; set; }
		}

		[Test]
		public void AndAlso ()
		{
			var predicate = PredicateBuilder.True<int> ().AndAlso (PredicateBuilder.True<int> ());

			AssertExpression (@"
bool (int var_$0)
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
bool (int var_$0)
{
	return true || false;
}
", predicate);
		}

		[Test]
		public void Not ()
		{
			var predicate = PredicateBuilder.True<int> ().Not ();

			AssertExpression (@"
bool (int var_$0)
{
	return !true;
}
", predicate);
		}

		[Test]
		public void FooAndBar ()
		{
			Expression<Func<User, bool>> is_foo = u => u.IsFoo;
			Expression<Func<User, bool>> is_bar = u => u.IsBar;

			var predicate = is_foo.AndAlso (is_bar);

			AssertExpression (@"
bool (User u)
{
	return u.IsFoo && u.IsBar;
}
", predicate);
		}
	}
}
