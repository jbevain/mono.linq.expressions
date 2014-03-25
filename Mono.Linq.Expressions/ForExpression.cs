//
// ForExpression.cs
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
// the following tests:
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

	public class ForExpression : CustomExpression {

		readonly ParameterExpression variable;
		readonly Expression initializer;
		readonly Expression test;
		readonly Expression step;

		readonly Expression body;

		readonly LabelTarget break_target;
		readonly LabelTarget continue_target;

		public new ParameterExpression Variable {
			get { return variable; }
		}

		public Expression Initializer {
			get { return initializer; }
		}

		public Expression Test {
			get { return test; }
		}

		public Expression Step {
			get { return step; }
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
			get { return CustomExpressionType.ForExpression; }
		}

		internal ForExpression (ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			this.variable = variable;
			this.initializer = initializer;
			this.test = test;
			this.step = step;
			this.body = body;
			this.break_target = breakTarget;
			this.continue_target = continueTarget;
		}

		public ForExpression Update (ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (this.variable == variable && this.initializer == initializer && this.test == test && this.step == step && this.body == body && this.break_target == breakTarget && this.continue_target == continueTarget)
				return this;

			return CustomExpression.For (variable, initializer, test, step, body, breakTarget, continueTarget);
		}

		public override Expression Reduce ()
		{
			var inner_loop_break = Expression.Label ("inner_loop_break");
			var inner_loop_continue = Expression.Label ("inner_loop_continue");

			var @continue = continue_target ?? Expression.Label ("continue");
			var @break = break_target ?? Expression.Label ("break");

			return Expression.Block (
				new [] { variable },
				variable.Assign (initializer),
				Expression.Loop (
					Expression.Block (
						Expression.IfThen (
							Expression.IsFalse (test),
							Expression.Break (inner_loop_break)),
						body,
						Expression.Label (@continue),
						step),
					inner_loop_break,
					inner_loop_continue),
				Expression.Label (@break));
		}

		protected override Expression VisitChildren (ExpressionVisitor visitor)
		{
			return Update (
				(ParameterExpression) visitor.Visit (variable),
				visitor.Visit (initializer),
				visitor.Visit (test),
				visitor.Visit (step),
				visitor.Visit (body),
				break_target,
				continue_target);
		}

		public override Expression Accept (CustomExpressionVisitor visitor)
		{
			return visitor.VisitForExpression (this);
		}
	}

	public abstract partial class CustomExpression {

		public static ForExpression For (ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body)
		{
			return For (variable, initializer, test, step, body, null);
		}

		public static ForExpression For (ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget)
		{
			return For (variable, initializer, test, step, body, breakTarget, null);
		}

		public static ForExpression For (ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (variable == null)
				throw new ArgumentNullException ("variable");
			if (initializer == null)
				throw new ArgumentNullException ("initializer");
			if (test == null)
				throw new ArgumentNullException ("test");
			if (step == null)
				throw new ArgumentNullException ("step");
			if (body == null)
				throw new ArgumentNullException ("body");

			if (!variable.Type.IsAssignableFrom (initializer.Type))
				throw new ArgumentException ("Initializer must be assignable to variable", "initializer");

			if (test.Type != typeof (bool))
				throw new ArgumentException ("Test must be a boolean expression", "test");

			if (continueTarget != null && continueTarget.Type != typeof (void))
				throw new ArgumentException ("Continue label target must be void", "continueTarget");

			return new ForExpression (variable, initializer, test, step, body, breakTarget, continueTarget);
		}
	}
}
