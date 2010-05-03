using System.Linq.Expressions;

namespace Mono.Linq.Expressions {

	public interface IExpressionWriter {
		void Write (Expression expression);
		void Write (ElementInit initializer);
		void Write (MemberBinding binding);
		void Write (CatchBlock block);
		void Write (SwitchCase @case);
		void Write (LabelTarget target);
	}
}
