using System;
using System.Linq.Expressions;

[AttributeUsage (AttributeTargets.GenericParameter)]
class DelegateConstraintAttribute : Attribute {
}

namespace Mono.Linq.Expressions {

	public static class DelegateConverter {

		public static Expression<TDelegate> ToExpression<[DelegateConstraint] TDelegate> (this TDelegate @delegate)
			where TDelegate : class
		{
			throw new NotImplementedException ();
		}
	}
}
