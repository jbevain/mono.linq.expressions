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

		readonly ParameterExpression variable;
		readonly Expression disposable;
		readonly Expression body;

		public new ParameterExpression Variable {
			get { return variable; }
		}

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

		internal UsingExpression (ParameterExpression variable, Expression disposable, Expression body)
		{
			this.variable = variable;
			this.disposable = disposable;
			this.body = body;
		}

		public UsingExpression Update (ParameterExpression variable, Expression disposable, Expression body)
		{
			if (this.variable == variable && this.disposable == disposable && this.body == body)
				return this;

			return CustomExpression.Using (variable, disposable, body);
		}

		public override Expression Reduce ()
		{
			var end_finally = Expression.Label ("end_finally");

			return Expression.Block (
				new [] { variable },
				variable.Assign (disposable),
				Expression.TryFinally (
					body,
					Expression.Block (
						variable.NotEqual (Expression.Constant (null)).Condition (
							Expression.Block (
								Expression.Call (
									variable.Convert (typeof (IDisposable)),
									typeof (IDisposable).GetMethod ("Dispose")),
								Expression.Goto (end_finally)),
							Expression.Goto (end_finally)),
						Expression.Label (end_finally))));
		}

		protected override Expression VisitChildren (ExpressionVisitor visitor)
		{
			return Update (
				(ParameterExpression) visitor.Visit (variable),
				visitor.Visit (disposable),
				visitor.Visit (body));
		}

		public override Expression Accept (CustomExpressionVisitor visitor)
		{
			return visitor.VisitUsingExpression (this);
		}
	}

	public abstract partial class CustomExpression {

		public static UsingExpression Using (Expression disposable, Expression body)
		{
			return Using (null, disposable, body);
		}

		public static UsingExpression Using (ParameterExpression variable, Expression disposable, Expression body)
		{
			if (disposable == null)
				throw new ArgumentNullException ("disposable");
			if (body == null)
				throw new ArgumentNullException ("body");

			if (!typeof (IDisposable).IsAssignableFrom (disposable.Type))
				throw new ArgumentException ("The disposable must implement IDisposable", "disposable");

			if (variable == null)
				variable = Expression.Parameter (disposable.Type);

			return new UsingExpression (variable, disposable, body);
		}
	}
}
