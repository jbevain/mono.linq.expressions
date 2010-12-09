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
