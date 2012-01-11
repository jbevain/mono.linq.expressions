//
// CombineExtensions.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2012 Jb Evain
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mono.Linq.Expressions {

	public static class CombineExtensions {

		public static Expression<T> Combine<[DelegateConstraint] T> (this Expression<T> self, Func<Expression, Expression> combinator) where T : class
		{
			if (self == null)
				throw new ArgumentNullException ("self");
			if (combinator == null)
				throw new ArgumentNullException ("combinator");

			var parameters = ParametersFor(self);

			return Expression.Lambda<T> (combinator (RewriteBody (self, parameters)), parameters);
		}

		public static Expression<T> Combine<[DelegateConstraint] T> (this Expression<T> self, Expression<T> expression, Func<Expression, Expression, Expression> combinator) where T : class
		{
			if (self == null)
				throw new ArgumentNullException ("self");
			if (expression == null)
				throw new ArgumentNullException ("expression");
			if (combinator == null)
				throw new ArgumentNullException ("combinator");

			var parameters = ParametersFor (self);

			return Expression.Lambda<T> (combinator (RewriteBody (self, parameters), RewriteBody (expression, parameters)), parameters);
		}

		static ParameterExpression [] ParametersFor (LambdaExpression lambda)
		{
			return lambda.Parameters.Select (p => Expression.Parameter (p.Type, p.Name)).ToArray ();
		}

		static Expression RewriteBody (LambdaExpression expression, IEnumerable<ParameterExpression> parameters)
		{
			return new ParameterRewriter (expression.Parameters, parameters).Visit (expression.Body);
		}

		class ParameterRewriter : ExpressionVisitor {

			readonly IDictionary<ParameterExpression, ParameterExpression> parameterMapping;

			public ParameterRewriter (IEnumerable<ParameterExpression> candidates, IEnumerable<ParameterExpression> replacements)
			{
				parameterMapping = ParametersMappingFor (candidates, replacements);
			}

			static IDictionary<ParameterExpression, ParameterExpression> ParametersMappingFor (IEnumerable<ParameterExpression> candidates, IEnumerable<ParameterExpression> replacements)
			{
				return candidates.Zip (replacements, (candidate, replacement) => new { candidate, replacement }).ToDictionary (t => t.candidate, t => t.replacement);
			}

			protected override Expression VisitParameter (ParameterExpression expression)
			{
				ParameterExpression replacement;
				return parameterMapping.TryGetValue(expression, out replacement) ? replacement : expression;
			}
		}

	}
}
