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

	public class ForExpression : Expression {

		readonly ParameterExpression variable;
		readonly Expression initializer;
		readonly Expression condition;
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

		public new Expression Condition {
			get { return condition; }
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

		public override ExpressionType NodeType {
			get { return ExpressionType.Extension; }
		}

		public override Type Type {
			get { return body.Type; }
		}

		public override bool CanReduce {
			get { return true; }
		}

		internal ForExpression (ParameterExpression variable, Expression initializer, Expression condition, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			this.variable = variable;
			this.initializer = initializer;
			this.condition = condition;
			this.step = step;
			this.body = body;
			this.break_target = breakTarget;
			this.continue_target = continueTarget;
		}

		public ForExpression Update (ParameterExpression variable, Expression initializer, Expression condition, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (this.variable == variable && this.initializer == initializer && this.condition == condition && this.step == step && this.body == body && this.break_target == breakTarget && this.continue_target == continueTarget)
				return this;

			return Create (variable, initializer, condition, step, body, breakTarget, continueTarget);
		}

		public override Expression Reduce ()
		{
			var inner_loop_break = Expression.Label ("inner_loop_break");
			var inner_loop_continue = Expression.Label ("inner_loop_continue");

			return Expression.Block (
				new [] { variable },
				initializer,
				Expression.Loop (
					Expression.Block (
						body,
						Expression.Label (continue_target ?? Expression.Label ("for_continue")),
						step,
						Expression.Condition (
							condition,
							Expression.Continue (inner_loop_continue),
							Expression.Break (inner_loop_break))),
					inner_loop_break,
					inner_loop_continue),
				Expression.Label (break_target ?? Expression.Label ("for_break")));
		}

		protected override Expression VisitChildren (ExpressionVisitor visitor)
		{
			return Update (
				(ParameterExpression) visitor.Visit (variable),
				visitor.Visit (initializer),
				visitor.Visit (condition),
				visitor.Visit (step),
				visitor.Visit (body),
				continue_target,
				break_target);
		}

		public static ForExpression Create (ParameterExpression variable, Expression initializer, Expression condition, Expression step, Expression body)
		{
			return Create (variable, initializer, condition, step, body, null);
		}

		public static ForExpression Create (ParameterExpression variable, Expression initializer, Expression condition, Expression step, Expression body, LabelTarget breakTarget)
		{
			return Create (variable, initializer, condition, step, body, breakTarget, null);
		}

		public static ForExpression Create (ParameterExpression variable, Expression initializer, Expression condition, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (variable == null)
				throw new ArgumentNullException ("variable");
			if (initializer == null)
				throw new ArgumentNullException ("initializer");
			if (condition == null)
				throw new ArgumentNullException ("condition");
			if (step == null)
				throw new ArgumentNullException ("step");
			if (body == null)
				throw new ArgumentNullException ("body");

			if (condition.Type != typeof (bool))
				throw new ArgumentException ("Condition must be a boolean expression", "condition");

			if (continueTarget != null && continueTarget.Type != typeof (void))
				throw new ArgumentException ("Continue label target must be void", "continueTarget");

			return new ForExpression (variable, initializer, condition, step, body, breakTarget, continueTarget);
		}
	}
}
