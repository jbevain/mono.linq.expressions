//
// UsingExpressionTest.cs
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
	public class UsingExpressionTest : BaseExpressionTest {

		public class Disposable : IDisposable {

			bool disposed;
			bool touched;

			public bool Disposed {
				get { return disposed; }
			}

			public bool Touched {
				get { return touched; }
			}

			public void Touch ()
			{
				touched = true;
			}

			void IDisposable.Dispose ()
			{
				disposed = true;
			}
		}

		[Test]
		public void Using ()
		{
			var disposable = new Disposable ();

			var d = Expression.Parameter (typeof (Disposable), "d");

			var disposer = Expression.Lambda<Action<Disposable>> (
				UsingExpression.Create (
					d,
					Expression.Block (
						Expression.Call (d, typeof (Disposable).GetMethod ("Touch")))),
				d).Compile ();

			Assert.IsFalse (disposable.Touched);
			Assert.IsFalse (disposable.Disposed);

			disposer (disposable);

			Assert.IsTrue (disposable.Touched);
			Assert.IsTrue (disposable.Disposed);
		}

		class TestUsingException : Exception {
		}

		[Test]
		public void UsingException ()
		{
			var disposable = new Disposable ();

			var d = Expression.Parameter (typeof (Disposable), "d");

			var disposer = Expression.Lambda<Action<Disposable>> (
				UsingExpression.Create (
					d,
					Expression.Block (
						Expression.Throw (Expression.New (typeof (TestUsingException))),
						Expression.Call (d, typeof (Disposable).GetMethod ("Touch")))),
				d).Compile ();

			Assert.IsFalse (disposable.Touched);
			Assert.IsFalse (disposable.Disposed);

			try {
				disposer (disposable);
				Assert.Fail ();
			} catch (TestUsingException) {}

			Assert.IsFalse (disposable.Touched);
			Assert.IsTrue (disposable.Disposed);
		}
	}
}
