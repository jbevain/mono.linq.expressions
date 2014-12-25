//
// ExpressionWriter.cs
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

	public abstract class ExpressionWriter : CustomExpressionVisitor, IExpressionWriter {

		readonly IFormatter formatter;

		protected ExpressionWriter (IFormatter formatter)
		{
			this.formatter = formatter;
		}

		public virtual void Write (LambdaExpression expression)
		{
			Visit (expression.Body);
		}

		public virtual void Write (Expression expression)
		{
			Visit (expression);
		}

        public virtual void Write(ExpressionType expressionType)
        {
            WriteSpace();
            switch (expressionType)
            {
                case ExpressionType.Add:
                    Write("+");
                    break;
                case ExpressionType.AddChecked:
                    Write("+");
                    break;
                case ExpressionType.And:
                    Write("&");
                    break;
                case ExpressionType.AndAlso:
                    Write("&&");
                    break;
                case ExpressionType.Coalesce:
                    Write("??");
                    break;
                case ExpressionType.Divide:
                    Write("/");
                    break;
                case ExpressionType.Equal:
                    Write("==");
                    break;
                case ExpressionType.ExclusiveOr:
                    Write("^");
                    break;
                case ExpressionType.GreaterThan:
                    Write(">");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    Write(">=");
                    break;
                case ExpressionType.LeftShift:
                    Write("<<");
                    break;
                case ExpressionType.LessThan:
                    Write("<");
                    break;
                case ExpressionType.LessThanOrEqual:
                    Write("<=");
                    break;
                case ExpressionType.Modulo:
                    Write("%");
                    break;
                case ExpressionType.Multiply:
                    Write("*");
                    break;
                case ExpressionType.MultiplyChecked:
                    Write("*");
                    break;
                case ExpressionType.Negate:
                    Write("-");
                    break;
                case ExpressionType.UnaryPlus:
                    Write("+");
                    break;
                case ExpressionType.NegateChecked:
                    Write("-");
                    break;
                case ExpressionType.Not:
                    Write("!");
                    break;
                case ExpressionType.NotEqual:
                    Write("!=");
                    break;
                case ExpressionType.Or:
                    Write("|");
                    break;
                case ExpressionType.OrElse:
                    Write("||");
                    break;
                case ExpressionType.RightShift:
                    Write(">>");
                    break;
                case ExpressionType.Subtract:
                    Write("-");
                    break;
                case ExpressionType.SubtractChecked:
                    Write("-");
                    break;
                case ExpressionType.Assign:
                    Write("=");
                    break;
                case ExpressionType.Decrement:
                    Write("--");
                    break;
                case ExpressionType.Increment:
                    Write("++");
                    break;
                case ExpressionType.AddAssign:
                    Write("+=");
                    break;
                case ExpressionType.AndAssign:
                    Write("&=");
                    break;
                case ExpressionType.DivideAssign:
                    Write("/=");
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    Write("^=");
                    break;
                case ExpressionType.LeftShiftAssign:
                    Write("<<=");
                    break;
                case ExpressionType.ModuloAssign:
                    Write("%=");
                    break;
                case ExpressionType.MultiplyAssign:
                    Write("*=");
                    break;
                case ExpressionType.OrAssign:
                    Write("|=");
                    break;
                case ExpressionType.RightShiftAssign:
                    Write(">>=");
                    break;
                case ExpressionType.SubtractAssign:
                    Write("-=");
                    break;
                case ExpressionType.AddAssignChecked:
                    Write("+=");
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    Write("*=");
                    break;
                case ExpressionType.SubtractAssignChecked:
                    Write("-=");
                    break;
                case ExpressionType.OnesComplement:
                    Write("~");
                    break;
            }
            WriteSpace();
        }

		public virtual void Write (ElementInit initializer)
		{
			VisitElementInit (initializer);
		}

		public virtual void Write (MemberBinding binding)
		{
			VisitMemberBinding (binding);
		}

		public virtual void Write (CatchBlock block)
		{
			VisitCatchBlock (block);
		}

		public virtual void Write (SwitchCase @case)
		{
			VisitSwitchCase (@case);
		}

		public virtual void Write (LabelTarget target)
		{
			VisitLabelTarget (target);
		}

		protected void Write (string str)
		{
			formatter.Write (str);
		}

		protected void WriteLine ()
		{
			formatter.WriteLine ();
		}

		protected void WriteSpace ()
		{
			formatter.WriteSpace ();
		}

		protected void WriteToken (string token)
		{
			formatter.WriteToken (token);
		}

		protected void WriteKeyword (string keyword)
		{
			formatter.WriteKeyword (keyword);
		}

		protected void WriteLiteral (string literal)
		{
			formatter.WriteLiteral (literal);
		}

		protected void WriteReference (string value, object reference)
		{
			formatter.WriteReference (value, reference);
		}

		protected void WriteIdentifier (string value, object identifier)
		{
			formatter.WriteIdentifier (value, identifier);
		}

		protected void Indent ()
		{
			formatter.Indent ();
		}

		protected void Dedent ()
		{
			formatter.Dedent ();
		}
	}
}
