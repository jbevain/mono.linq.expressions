using System;
using System.Linq.Expressions;

// inspired by http://www.albahari.com/nutshell/predicatebuilder.aspx

namespace Mono.Linq.Expressions {

	public static class PredicateBuilder {

		public static Expression<Func<T, bool>> True<T> ()
		{
			return Expression.Lambda<Func<T, bool>> (Expression.Constant (true), Expression.Parameter (typeof (T)));
		}

		public static Expression<Func<T, bool>> False<T> ()
		{
			return Expression.Lambda<Func<T, bool>> (Expression.Constant (false), Expression.Parameter (typeof (T)));
		}

		public static Expression<Func<T, bool>> OrElse<T> (this Expression<Func<T, bool>> self, Expression<Func<T, bool>> expression)
		{
			return Combine (self, expression, Expression.OrElse);
		}

		public static Expression<Func<T, bool>> AndAlso<T> (this Expression<Func<T, bool>> self, Expression<Func<T, bool>> expression)
		{
			return Combine (self, expression, Expression.AndAlso);
		}

		static Expression<Func<T, bool>> Combine<T> (Expression<Func<T, bool>> self, Expression<Func<T, bool>> expression, Func<Expression, Expression, Expression> selector)
		{
			CheckSelfAndExpression (self, expression);

			var parameter = CreateParameterFrom (self);

			return Expression.Lambda<Func<T, bool>> (
				selector (
					RewriteFor (self.Body, parameter),
					RewriteFor (expression.Body, parameter)),
				parameter);
		}

		static Expression RewriteFor (Expression expression, ParameterExpression parameter)
		{
			return new ParameterRewriter (parameter).Visit (expression);
		}

		class ParameterRewriter : ExpressionVisitor {

			readonly ParameterExpression parameter;

			public ParameterRewriter (ParameterExpression parameter)
			{
				this.parameter = parameter;
			}

			protected override Expression VisitParameter (ParameterExpression expression)
			{
				return parameter;
			}
		}

		static ParameterExpression CreateParameterFrom<T> (Expression<Func<T, bool>> left)
		{
			var template = left.Parameters [0];

			return Expression.Parameter (template.Type, template.Name);
		}

		static void CheckSelfAndExpression<T> (Expression<Func<T, bool>> self, Expression<Func<T, bool>> expression)
		{
			if (self == null)
				throw new ArgumentNullException ("self");
			if (expression == null)
				throw new ArgumentNullException ("expression");
		}
	}
}
