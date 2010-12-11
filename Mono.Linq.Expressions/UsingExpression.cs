//
// UsingExpression.cs
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

namespace Mono.Linq.Expressions {

	public class UsingExpression : CustomExpression {

		readonly Expression disposable;
		readonly Expression body;

		public Expression Disposable {
			get { return disposable; }
		}

		public Expression Body {
			get { return body; }
		}

		public override Type Type {
			get { return body.Type; }
		}

		public override CustomExpressionType CustomNodeType {
			get { return CustomExpressionType.UsingExpression; }
		}

		internal UsingExpression (Expression disposable, Expression body)
		{
			this.disposable = disposable;
			this.body = body;
		}

		public UsingExpression Update (Expression disposable, Expression body)
		{
			if (this.disposable == disposable && this.body == body)
				return this;

			return Create (disposable, body);
		}

		public override Expression Reduce ()
		{
			var end_finally = Expression.Label ("end_finally");

			var variable = Expression.Variable (typeof (IDisposable));

			return Expression.Block (
				new [] { variable },
				Expression.Assign (variable, Expression.Convert (disposable, typeof (IDisposable))),
				Expression.TryFinally (
					body,
					Expression.Block (
						Expression.Condition (
							Expression.NotEqual (variable, Expression.Constant (null)),
							Expression.Block (
								Expression.Call (
									variable,
									typeof (IDisposable).GetMethod ("Dispose")),
								Expression.Goto (end_finally)),
							Expression.Goto (end_finally)),
						Expression.Label (end_finally))));
		}

		protected override Expression VisitChildren (ExpressionVisitor visitor)
		{
			return Update (
				visitor.Visit (disposable),
				visitor.Visit (body));
		}

		public override Expression Accept (CustomExpressionVisitor visitor)
		{
			return visitor.VisitUsingExpression (this);
		}

		public static UsingExpression Create (Expression disposable, Expression body)
		{
			if (disposable == null)
				throw new ArgumentNullException ("disposable");
			if (body == null)
				throw new ArgumentNullException ("body");

			if (!typeof (IDisposable).IsAssignableFrom (disposable.Type))
				throw new ArgumentException ("The disposable must implement IDisposable", "disposable");

			return new UsingExpression (disposable, body);
		}
	}
}
