//
// ForEachExpression.cs
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

namespace Mono.Linq.Expressions {

	public class ForEachExpression : Expression {

		readonly ParameterExpression variable;
		readonly Expression enumerable;
		readonly Expression body;

		readonly LabelTarget break_target;
		readonly LabelTarget continue_target;

		public new ParameterExpression Variable {
			get { return variable; }
		}

		public Expression Enumerable {
			get { return enumerable; }
		}

		public Expression Body {
			get { return body; }
		}

		public LabelTarget BreakTarget {
			get { return break_target; }
		}

		public LabelTarget ContinueTarget {
			get { return continue_target; }
		}

		public override ExpressionType NodeType {
			get { return ExpressionType.Extension; }
		}

		public override bool CanReduce {
			get { return true; }
		}

		public override Type Type {
			get { return body.Type; }
		}

		internal ForEachExpression (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget break_target, LabelTarget continue_target)
		{
			this.variable = variable;
			this.enumerable = enumerable;
			this.body = body;
			this.break_target = break_target;
			this.continue_target = continue_target;
		}

		public ForEachExpression Update (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (this.variable == variable && this.enumerable == enumerable && this.body == body && break_target == breakTarget && continue_target == continueTarget)
				return this;

			return Create (variable, enumerable, body, continueTarget, breakTarget);
		}

		public override Expression Reduce ()
		{
			Type argument;
			Type enumerable_type;
			Type enumerator_type;
			if (!TryGetGenericEnumerableArgument (out argument)) {
				enumerator_type = typeof (IEnumerator);
				enumerable_type = typeof (IEnumerable);
			} else {
				enumerator_type = typeof (IEnumerator<>).MakeGenericType (argument);
				enumerable_type = typeof (IEnumerable<>).MakeGenericType (argument);
			}

			var enumerator = Expression.Variable (enumerator_type);
			var disposable = Expression.Variable (typeof (IDisposable));

			var inner_loop_continue = Expression.Label ("inner_loop_continue");
			var inner_loop_break = Expression.Label ("inner_loop_break");
			var @continue = continue_target ?? Expression.Label ("continue");
			var @break = break_target ?? Expression.Label ("break");
			var end_finally = Expression.Label ("end");

			return Expression.Block (
				new[] {enumerator},
				Expression.Assign (
					enumerator,
					Expression.Call (
						Expression.Convert (enumerable, enumerable_type),
						enumerable_type.GetMethod ("GetEnumerator", Type.EmptyTypes))),
				Expression.TryFinally (
					Expression.Block (
						new[] { variable },
						Expression.Goto (@continue),
						Expression.Loop (
							Expression.Block (
								Expression.Assign (
									variable,
									Expression.Convert (
										Expression.Property (
											enumerator,
											"Current"),
										variable.Type)),
									body,
									Expression.Label (@continue),
									Expression.Condition (
										Expression.Call (
											enumerator,
											typeof (IEnumerator).GetMethod ("MoveNext", Type.EmptyTypes)),
										Expression.Goto (inner_loop_continue),
										Expression.Goto (inner_loop_break))),
							inner_loop_break,
							inner_loop_continue),
						Expression.Label (@break)),
					Expression.Block (
						new [] { disposable },
						Expression.Assign (
							disposable,
							Expression.TypeAs (enumerator, typeof (IDisposable))),
							Expression.Condition (
								Expression.NotEqual (
									disposable,
									Expression.Constant (null)),
								Expression.Call (disposable, typeof (IDisposable).GetMethod ("Dispose", Type.EmptyTypes)),
								Expression.Goto (end_finally)),
							Expression.Label (end_finally))));
		}

		bool TryGetGenericEnumerableArgument (out Type argument)
		{
			argument = null;

			foreach (var iface in enumerable.Type.GetInterfaces ()) {
				if (!iface.IsGenericType)
					continue;

				var definition = iface.GetGenericTypeDefinition ();
				if (definition != typeof (IEnumerable<>))
					continue;

				argument = iface.GetGenericArguments () [0];
				if (variable.Type.IsAssignableFrom (argument))
					return true;
			}

			return false;
		}

		protected override Expression VisitChildren (ExpressionVisitor visitor)
		{
			return Update (
				(ParameterExpression) visitor.Visit (variable),
				visitor.Visit (enumerable),
				visitor.Visit (body),
				break_target,
				continue_target);
		}

		public static ForEachExpression Create (ParameterExpression variable, Expression enumerable, Expression body)
		{
			return Create (variable, enumerable, body, null);
		}

		public static ForEachExpression Create (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget)
		{
			return Create (variable, enumerable, body, breakTarget, null);
		}

		public static ForEachExpression Create (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (variable == null)
				throw new ArgumentNullException ("variable");
			if (enumerable == null)
				throw new ArgumentNullException ("enumerable");
			if (body == null)
				throw new ArgumentNullException ("body");

			if (!typeof (IEnumerable).IsAssignableFrom (enumerable.Type))
				throw new ArgumentException ("The enumerable must implement at least IEnumerable", "enumerable");

			if (continueTarget != null && continueTarget.Type != typeof (void))
				throw new ArgumentException ("Continue label target must be void", "continueTarget");

			return new ForEachExpression (variable, enumerable, body, breakTarget, continueTarget);
		}
	}
}
