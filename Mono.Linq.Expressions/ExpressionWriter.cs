using System.Linq.Expressions;

namespace Mono.Linq.Expressions {

	public abstract class ExpressionWriter : ExpressionVisitor, IExpressionWriter {

		readonly IFormatter formatter;

		protected ExpressionWriter (IFormatter formatter)
		{
			this.formatter = formatter;
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
