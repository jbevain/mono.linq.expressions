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
using System.Reflection;

namespace Mono.Linq.Expressions {

	public class ForEachExpression : CustomExpression {

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

		public override Type Type {
			get {
				if (break_target != null)
					return break_target.Type;

				return typeof (void);
			}
		}

		public override CustomExpressionType CustomNodeType {
			get { return CustomExpressionType.ForEachExpression; }
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

			return CustomExpression.ForEach (variable, enumerable, body, continueTarget, breakTarget);
		}

		public override Expression Reduce ()
		{
			// Avoid allocating an unnecessary enumerator for arrays.
			if (enumerable.Type.IsArray)
				return ReduceForArray ();

			return ReduceForEnumerable ();
		}

		private Expression ReduceForArray ()
		{
			var inner_loop_break = Expression.Label ("inner_loop_break");
			var inner_loop_continue = Expression.Label ("inner_loop_continue");

			var @continue = continue_target ?? Expression.Label ("continue");
			var @break = break_target ?? Expression.Label ("break");

			var index = Expression.Variable (typeof (int), "i");

			return Expression.Block (
				new [] { index, variable },
				index.Assign (Expression.Constant (0)),
				Expression.Loop (
					Expression.Block (
						Expression.IfThen (
							Expression.IsFalse (
								Expression.LessThan (
									index,
									Expression.ArrayLength (enumerable))),
							Expression.Break (inner_loop_break)),
						variable.Assign (
							Expression.ArrayIndex (
								enumerable,
								index).Convert (variable.Type)),
						body,
						Expression.Label (@continue),
						Expression.PreIncrementAssign (index)),
					inner_loop_break,
					inner_loop_continue),
				Expression.Label (@break));
		}

		private Expression ReduceForEnumerable ()
		{
			MethodInfo get_enumerator;
			MethodInfo move_next;
			MethodInfo get_current;

			ResolveEnumerationMembers (out get_enumerator, out move_next, out get_current);

			var enumerator_type = get_enumerator.ReturnType;

			var enumerator = Expression.Variable (enumerator_type);

			var inner_loop_continue = Expression.Label ("inner_loop_continue");
			var inner_loop_break = Expression.Label ("inner_loop_break");
			var @continue = continue_target ?? Expression.Label ("continue");
			var @break = break_target ?? Expression.Label ("break");

			Expression variable_initializer;

			if (variable.Type.IsAssignableFrom (get_current.ReturnType))
				variable_initializer = enumerator.Property (get_current);
			else
				variable_initializer = enumerator.Property (get_current).Convert (variable.Type);

			Expression loop = Expression.Block (
				new [] { variable },
				Expression.Goto (@continue),
				Expression.Loop (
					Expression.Block (
						variable.Assign (variable_initializer),
						body,
						Expression.Label (@continue),
						Expression.Condition (
							Expression.Call (enumerator, move_next),
							Expression.Goto (inner_loop_continue),
							Expression.Goto (inner_loop_break))),
					inner_loop_break,
					inner_loop_continue),
				Expression.Label (@break));

			var dispose = CreateDisposeOperation (enumerator_type, enumerator);

			return Expression.Block (
				new [] { enumerator },
				enumerator.Assign (Expression.Call (enumerable, get_enumerator)),
				dispose != null
					? Expression.TryFinally (loop, dispose)
					: loop);
		}

		private void ResolveEnumerationMembers (
			out MethodInfo get_enumerator,
			out MethodInfo move_next,
			out MethodInfo get_current)
		{
			Type item_type;
			Type enumerable_type;
			Type enumerator_type;

			if (TryGetGenericEnumerableArgument (out item_type)) {
				enumerable_type = typeof (IEnumerable<>).MakeGenericType (item_type);
				enumerator_type = typeof (IEnumerator<>).MakeGenericType (item_type);
			} else {
				enumerable_type = typeof (IEnumerable);
				enumerator_type = typeof (IEnumerator);
			}

			move_next = typeof (IEnumerator).GetMethod ("MoveNext");
			get_current = enumerator_type.GetProperty ("Current").GetGetMethod ();
			get_enumerator = enumerable.Type.GetMethod ("GetEnumerator", BindingFlags.Public | BindingFlags.Instance);

			//
			// We want to avoid unnecessarily boxing an enumerator if it's a value type.  Look
			// for a GetEnumerator() method that conforms to the rules of the C# 'foreach'
			// pattern.  If we don't find one, fall back to IEnumerable[<T>].GetEnumerator().
			//

			if (get_enumerator == null || !enumerator_type.IsAssignableFrom (get_enumerator.ReturnType)) {
				get_enumerator = enumerable_type.GetMethod ("GetEnumerator");
			}
		}

		private static Expression CreateDisposeOperation (Type enumerator_type, ParameterExpression enumerator)
		{
			var dispose = typeof (IDisposable).GetMethod ("Dispose");

			if (typeof (IDisposable).IsAssignableFrom (enumerator_type)) {
				//
				// We know the enumerator implements IDisposable, so skip the type check.
				//
				return Expression.Call (enumerator, dispose);
			}

			if (enumerator_type.IsValueType) {
				//
				// The enumerator is a value type and doesn't implement IDisposable; we needn't
				// bother with a check at all.
				//
				return null;
			}

			//
			// We don't know whether the enumerator implements IDisposable or not.  Emit a
			// runtime check.
			//

			var disposable = Expression.Variable (typeof (IDisposable));

			return Expression.Block (
				new [] { disposable },
				disposable.Assign (enumerator.TypeAs (typeof (IDisposable))),
				disposable.ReferenceNotEqual (Expression.Constant (null)).IfThen (
					Expression.Call (
						disposable,
						"Dispose",
						Type.EmptyTypes)));
		}

		private bool TryGetGenericEnumerableArgument (out Type argument)
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

		public override Expression Accept (CustomExpressionVisitor visitor)
		{
			return visitor.VisitForEachExpression (this);
		}
	}

	public abstract partial class CustomExpression {

		public static ForEachExpression ForEach (ParameterExpression variable, Expression enumerable, Expression body)
		{
			return ForEach (variable, enumerable, body, null);
		}

		public static ForEachExpression ForEach (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget)
		{
			return ForEach (variable, enumerable, body, breakTarget, null);
		}

		public static ForEachExpression ForEach (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
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
