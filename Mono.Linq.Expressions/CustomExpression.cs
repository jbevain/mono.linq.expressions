//
// CustomExpression.cs
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

using System.Linq.Expressions;

namespace Mono.Linq.Expressions {

	public enum CustomExpressionType {
		ForEachExpression,
		ForExpression,
		UsingExpression,
		WhileExpression,
	}

	public abstract class CustomExpression : Expression {

		public abstract CustomExpressionType CustomNodeType { get; }

		public abstract Expression Accept (CustomExpressionVisitor visitor);

		public static ForExpression For (ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body)
		{
			return ForExpression.Create (variable, initializer, test, step, body, null);
		}

		public static ForExpression For (ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget)
		{
			return ForExpression.Create (variable, initializer, test, step, body, breakTarget, null);
		}

		public static ForExpression For (ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			return ForExpression.Create (variable, initializer, test, step, body, breakTarget, continueTarget);
		}

		public static ForEachExpression ForEach (ParameterExpression variable, Expression enumerable, Expression body)
		{
			return ForEachExpression.Create (variable, enumerable, body, null);
		}

		public static ForEachExpression ForEach (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget)
		{
			return ForEachExpression.Create (variable, enumerable, body, breakTarget, null);
		}

		public static ForEachExpression ForEach (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			return ForEachExpression.Create (variable, enumerable, body, breakTarget, continueTarget);
		}

		public static UsingExpression Using (Expression disposable, Expression body)
		{
			return UsingExpression.Create (disposable, body);
		}

		public static WhileExpression While (Expression test, Expression body)
		{
			return WhileExpression.Create (test, body, null);
		}

		public static WhileExpression While (Expression test, Expression body, LabelTarget breakTarget)
		{
			return WhileExpression.Create (test, body, breakTarget, null);
		}

		public static WhileExpression While (Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			return WhileExpression.Create (test, body, breakTarget, continueTarget);
		}
	}
}
