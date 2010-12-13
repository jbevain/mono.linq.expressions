//
// WhileExpressionTest.cs
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
	public class WhileExpressionTest : BaseExpressionTest {

		public class Counter {

			int count;

			public int Count {
				get { return count; }
			}

			public void Hit ()
			{
				count++;
			}
		}

		[Test]
		public void While ()
		{
			var counter = new Counter ();

			var c = Expression.Parameter (typeof (Counter), "c");
			var i = Expression.Parameter (typeof (int), "i");
			var l = Expression.Parameter (typeof (int), "l");

			var hitcounter = Expression.Lambda<Action<Counter, int, int>> (
				CustomExpression.While (
					Expression.LessThan (i, l),
					Expression.Block (
						Expression.Call (c, typeof (Counter).GetMethod ("Hit", Type.EmptyTypes)),
						Expression.PreIncrementAssign (i))
					),
				c, i, l).Compile ();

			hitcounter (counter, 0, 10);

			Assert.AreEqual (10, counter.Count);
		}

		[Test]
		public void WhileFalse ()
		{
			var counter = new Counter ();

			var c = Expression.Parameter (typeof (Counter), "c");
			var i = Expression.Parameter (typeof (int), "i");
			var l = Expression.Parameter (typeof (int), "l");

			var hitcounter = Expression.Lambda<Action<Counter, int, int>> (
				CustomExpression.While (
					Expression.LessThan (i, l),
					Expression.Block (
						Expression.Call (c, typeof (Counter).GetMethod ("Hit", Type.EmptyTypes)),
						Expression.PreIncrementAssign (i))
					),
				c, i, l).Compile ();

			hitcounter (counter, 100, 10);

			Assert.AreEqual (0, counter.Count);
		}

		[Test]
		public void WhileBreakContinue ()
		{
			var counter = new Counter ();

			var c = Expression.Parameter (typeof (Counter), "c");
			var i = Expression.Parameter (typeof (int), "i");
			var l = Expression.Parameter (typeof (int), "l");

			var loop_break = Expression.Label ("for_break");
			var loop_continue = Expression.Label ("for_continue");

			var hitcounter = Expression.Lambda<Action<Counter, int, int>> (
				CustomExpression.While (
					Expression.LessThan (i, l),
					Expression.Block (
						Expression.Condition (
							Expression.LessThan (i, Expression.Constant (10)),
							Expression.Block (
								Expression.Call (c, typeof (Counter).GetMethod ("Hit", Type.EmptyTypes)),
								Expression.PostIncrementAssign (i),
								Expression.Goto (loop_continue)),
							Expression.Goto (loop_break))),
					loop_break,
					loop_continue),
				c, i, l).Compile ();

			hitcounter (counter, 0, 100);

			Assert.AreEqual (10, counter.Count);
		}
	}
}
