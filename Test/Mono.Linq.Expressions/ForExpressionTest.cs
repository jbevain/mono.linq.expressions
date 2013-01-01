//
// ForExpressionTest.cs
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
	public class ForExpressionTest : BaseExpressionTest {

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
		public void For ()
		{
			var counter = new Counter ();

			var c = Expression.Parameter (typeof (Counter), "c");
			var l = Expression.Parameter (typeof (int), "l");

			var i = Expression.Variable (typeof (int), "i");

			var hitcounter = Expression.Lambda<Action<Counter, int>> (
				CustomExpression.For (
					i,
					Expression.Constant (0),
					Expression.LessThan (i, l),
					Expression.PreIncrementAssign (i),
					Expression.Call (c, typeof (Counter).GetMethod ("Hit", Type.EmptyTypes))),
				c, l).Compile ();

			hitcounter (counter, 10);

			Assert.AreEqual (10, counter.Count);
		}

		[Test]
		public void Never ()
		{
			var counter = new Counter ();

			var c = Expression.Parameter (typeof (Counter), "c");
			var l = Expression.Parameter (typeof (int), "l");

			var i = Expression.Variable (typeof (int), "i");

			var hitcounter = Expression.Lambda<Action<Counter, int>> (
				CustomExpression.For (
					i,
					Expression.Constant (0),
					Expression.LessThan (i, l),
					Expression.PreIncrementAssign (i),
					Expression.Call (c, typeof (Counter).GetMethod ("Hit", Type.EmptyTypes))),
				c, l).Compile ();

			hitcounter (counter, 0);

			Assert.AreEqual (0, counter.Count);
		}

		[Test]
		public void ForBreak ()
		{
			var counter = new Counter ();

			var c = Expression.Parameter (typeof (Counter), "c");
			var l = Expression.Parameter (typeof (int), "l");

			var i = Expression.Variable (typeof (int), "i");
			var for_break = Expression.Label ("for_break");

			var hitcounter = Expression.Lambda<Action<Counter, int>> (
				CustomExpression.For (
					i,
					Expression.Constant (0),
					Expression.LessThan (i, l),
					Expression.PreIncrementAssign (i),
					Expression.Block (
						Expression.Condition (
							Expression.LessThan (i, Expression.Constant (10)),
							Expression.Call (c, typeof (Counter).GetMethod ("Hit", Type.EmptyTypes)),
							Expression.Goto (for_break))),
					for_break),
				c, l).Compile ();

			hitcounter (counter, 100);

			Assert.AreEqual (10, counter.Count);
		}

		[Test]
		public void ForContinue ()
		{
			var counter = new Counter ();

			var c = Expression.Parameter (typeof (Counter), "c");
			var l = Expression.Parameter (typeof (int), "l");

			var i = Expression.Variable (typeof (int), "i");
			var for_break = Expression.Label ("for_break");
			var for_continue = Expression.Label ("for_continue");

			var hitcounter = Expression.Lambda<Action<Counter, int>> (
				CustomExpression.For (
					i,
					Expression.Constant (0),
					Expression.LessThan (i, l),
					Expression.PreIncrementAssign (i),
					Expression.Block (
						Expression.Condition (
							Expression.Equal (Expression.Modulo (i, Expression.Constant (2)), Expression.Constant (0)),
							Expression.Call (c, typeof (Counter).GetMethod ("Hit", Type.EmptyTypes)),
							Expression.Goto (for_continue))),
					for_break,
					for_continue),
				c, l).Compile ();

			hitcounter (counter, 10);

			Assert.AreEqual (5, counter.Count);
		}
	}
}
