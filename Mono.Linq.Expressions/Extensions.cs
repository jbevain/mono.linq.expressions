using System;
using System.Linq.Expressions;

namespace Mono.Linq.Expressions {

	public static class Extensions {

		public static bool Is (this Expression self, ExpressionType type)
		{
			if (self == null)
				throw new ArgumentNullException ("self");

			return self.NodeType == type;
		}

		public static ConstantExpression ToConstant<T> (this T self)
		{
			return Expression.Constant (self, typeof (T));
		}
	}
}
