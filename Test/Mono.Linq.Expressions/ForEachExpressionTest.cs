//
// ForEachExpressionTest.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using NUnit.Framework;

namespace Mono.Linq.Expressions {

	[TestFixture]
	public class ForEachExpressionTest : BaseExpressionTest {

		public class EnumerableCounter : IEnumerable<int>, IEnumerator<int> {

			int count;
			int current;
			bool disposed;

			readonly int low;
			readonly int high;

			public int Count {
				get { return count; }
			}

			public bool Disposed {
				get { return disposed; }
			}

			int IEnumerator<int>.Current {
				get { return current; }
			}

			object IEnumerator.Current {
				get { return current; }
			}

			public EnumerableCounter (int low, int high)
			{
				this.low = low;
				this.high = high;
			}

			public void Hit ()
			{
				count++;
			}

			IEnumerator<int> IEnumerable<int>.GetEnumerator ()
			{
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return this;
			}

			bool IEnumerator.MoveNext ()
			{
				if (current == -1) {
					current = low;
					return true;
				}

				if (current == high)
					return false;

				current++;
				return true;
			}

			void IEnumerator.Reset ()
			{
				current = -1;
			}

			void IDisposable.Dispose ()
			{
				disposed = true;
			}
		}

		[Test]
		public void ForEach ()
		{
			var enumerable_counter = new EnumerableCounter (0, 10);

			var ec = Expression.Parameter (typeof (EnumerableCounter), "ec");

			var item = Expression.Variable (typeof (int), "i");

			var hitcounter = Expression.Lambda<Action<EnumerableCounter>> (
				ForEachExpression.Create (
					item,
					ec,
					Expression.Call (ec, typeof (EnumerableCounter).GetMethod ("Hit", Type.EmptyTypes))),
				ec).Compile ();

			hitcounter (enumerable_counter);

			Assert.AreEqual (10, enumerable_counter.Count);
			Assert.IsTrue (enumerable_counter.Disposed);
		}

		[Test]
		public void ForEachBreak ()
		{
			var enumerable_counter = new EnumerableCounter (0, 100);

			var ec = Expression.Parameter (typeof (EnumerableCounter), "ec");

			var item = Expression.Variable (typeof (int), "i");
			var foreach_break = Expression.Label ("foreach_break");

			var hitcounter = Expression.Lambda<Action<EnumerableCounter>> (
				ForEachExpression.Create (
					item,
					ec,
					Expression.Block (
						Expression.Condition (
							Expression.LessThanOrEqual (item, Expression.Constant (10)),
							Expression.Call (ec, typeof (EnumerableCounter).GetMethod ("Hit", Type.EmptyTypes)),
							Expression.Goto (foreach_break))),
					foreach_break),
				ec).Compile ();

			hitcounter (enumerable_counter);

			Assert.AreEqual (10, enumerable_counter.Count);
			Assert.IsTrue (enumerable_counter.Disposed);
		}

		[Test]
		public void ForEachContinue ()
		{
			var enumerable_counter = new EnumerableCounter (0, 10);

			var ec = Expression.Parameter (typeof (EnumerableCounter), "ec");

			var item = Expression.Variable (typeof (int), "i");
			var foreach_break = Expression.Label ("foreach_break");
			var foreach_continue = Expression.Label ("foreach_continue");

			var hitcounter = Expression.Lambda<Action<EnumerableCounter>> (
				ForEachExpression.Create (
					item,
					ec,
					Expression.Block (
						Expression.Condition (
							Expression.Equal (Expression.Modulo (item, Expression.Constant (2)), Expression.Constant (0)),
							Expression.Call (ec, typeof (EnumerableCounter).GetMethod ("Hit", Type.EmptyTypes)),
							Expression.Goto (foreach_continue))),
					foreach_break,
					foreach_continue),
				ec).Compile ();

			hitcounter (enumerable_counter);

			Assert.AreEqual (5, enumerable_counter.Count);
			Assert.IsTrue (enumerable_counter.Disposed);
		}

		class TestException : Exception {
		}

		[Test]
		public void ForEachException ()
		{
			var enumerable_counter = new EnumerableCounter (0, 100);

			var ec = Expression.Parameter (typeof (EnumerableCounter), "ec");

			var item = Expression.Variable (typeof (int), "i");

			var hitcounter = Expression.Lambda<Action<EnumerableCounter>> (
				ForEachExpression.Create (
					item,
					ec,
					Expression.Block (
						Expression.Condition (
							Expression.LessThanOrEqual (item, Expression.Constant (10)),
							Expression.Call (ec, typeof (EnumerableCounter).GetMethod ("Hit", Type.EmptyTypes)),
							Expression.Throw (Expression.New (typeof (TestException)))))),
				ec).Compile ();

			try {
				hitcounter (enumerable_counter);
				Assert.Fail ();
			} catch (TestException) {}

			Assert.AreEqual (10, enumerable_counter.Count);
			Assert.IsTrue (enumerable_counter.Disposed);
		}

		public class NonGenericEnumerableCounter : IEnumerable, IEnumerator {

			int count;
			int current;

			readonly int low;
			readonly int high;

			public int Count {
				get { return count; }
			}

			object IEnumerator.Current {
				get { return current; }
			}

			public NonGenericEnumerableCounter (int low, int high)
			{
				this.low = low;
				this.high = high;
			}

			public void Hit ()
			{
				count++;
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return this;
			}

			bool IEnumerator.MoveNext ()
			{
				if (current == -1) {
					current = low;
					return true;
				}

				if (current == high)
					return false;

				current++;
				return true;
			}

			void IEnumerator.Reset ()
			{
				current = -1;
			}
		}

		[Test]
		public void ForEachNonGeneric ()
		{
			var enumerable_counter = new NonGenericEnumerableCounter (0, 10);

			var ec = Expression.Parameter (typeof (NonGenericEnumerableCounter), "ec");

			var item = Expression.Variable (typeof (int), "i");

			var hitcounter = Expression.Lambda<Action<NonGenericEnumerableCounter>> (
				ForEachExpression.Create (
					item,
					ec,
					Expression.Call (ec, typeof (NonGenericEnumerableCounter).GetMethod ("Hit", Type.EmptyTypes))),
				ec).Compile ();

			hitcounter (enumerable_counter);

			Assert.AreEqual (10, enumerable_counter.Count);
		}
	}
}
