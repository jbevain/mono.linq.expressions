using System;
using System.Linq.Expressions;

namespace Mono.Linq.Expressions {

	public class CSharpWriter : ExpressionWriter {

		public CSharpWriter (IFormatter formatter)
			: base (formatter)
		{
		}

		protected override Expression VisitLambda<T> (Expression<T> node)
		{
			VisitLambdaSignature (node);
			VisitLambdaBody (node);

			return node;
		}

		void VisitLambdaSignature<T> (Expression<T> node)
		{
			WriteType (node.ReturnType);
			WriteSpace ();
			WriteIdentifier (node.Name, node);
			WriteToken ("(");
			for (int i = 0; i < node.Parameters.Count; i++) {
				if (i > 0) {
					WriteToken (",");
					WriteSpace ();
				}

				var parameter = node.Parameters [i];
				WriteType (parameter.Type);

				if (!string.IsNullOrEmpty (parameter.Name)) {
					WriteSpace ();
					WriteIdentifier (parameter.Name, parameter);
				}
			}
			WriteToken (")");
			WriteLine ();
		}

		void VisitLambdaBody<T> (Expression<T> node)
		{
			if (node.Body.NodeType != ExpressionType.Block)
				VisitSingleExpressionBody (node);
			else
				VisitBlockExpressionBody (node);
		}

		void VisitBlockExpressionBody<T> (Expression<T> node)
		{
			VisitBlockExpression ((BlockExpression) node.Body, node.ReturnType != typeof (void));
		}

		void VisitSingleExpressionBody<T> (Expression<T> node)
		{
			VisitBlock (() => {
				if (node.ReturnType != typeof (void)) {
					WriteKeyword ("return");
					WriteSpace ();
				}

				Visit (node.Body);

				WriteToken (";");
				WriteLine ();
			});
		}

		void WriteType (Type type)
		{
			WriteReference (GetTypeName (type), type);
		}

		static string GetTypeName (Type type)
		{
			switch (Type.GetTypeCode (type)) {
			case TypeCode.Boolean:
				return "bool";
			case TypeCode.Byte:
				return "byte";
			case TypeCode.Char:
				return "char";
			case TypeCode.Decimal:
				return "decimal";
			case TypeCode.Double:
				return "double";
			case TypeCode.Int16:
				return "short";
			case TypeCode.Int32:
				return "int";
			case TypeCode.Int64:
				return "long";
			case TypeCode.Object:
				return "object";
			case TypeCode.SByte:
				return "sbyte";
			case TypeCode.Single:
				return "float";
			case TypeCode.String:
				return "string";
			case TypeCode.UInt16:
				return "ushort";
			case TypeCode.UInt32:
				return "uint";
			case TypeCode.UInt64:
				return "ulong";
			default:
				return type.Name;
			}
		}

		protected override Expression VisitBlock (BlockExpression node)
		{
			VisitBlockExpression (node, false);

			return node;
		}

		void VisitBlock (Action action)
		{
			WriteToken ("{");
			WriteLine ();
			Indent ();

			action ();

			Dedent ();
			WriteToken ("}");
			WriteLine ();
		}

		void VisitBlockExpression (BlockExpression node, bool return_last)
		{
			VisitBlock (() => {
				foreach (var variable in node.Variables) {
					WriteType (variable.Type);
					WriteSpace ();
					WriteIdentifier (variable.Name, variable);
					WriteToken (";");
					WriteLine ();
				}

				if (node.Variables.Count > 0)
					WriteLine ();

				for (int i = 0; i < node.Expressions.Count; i++) {
					var expression = node.Expressions [i];

					if (return_last && i + 1 == node.Expressions.Count) {
						WriteKeyword ("return");
						WriteSpace ();
					}

					Write (expression);

					if (!IsActualStatement (expression))
						continue;

					WriteToken (";");
					WriteLine ();
				}
			});
		}

		static bool IsActualStatement (Expression expression)
		{
			switch (expression.NodeType) {
			case ExpressionType.Label:
				return false;
			default:
				return true;
			}
		}

		protected override Expression VisitBinary (BinaryExpression node)
		{
			if (IsChecked (node.NodeType))
				VisitCheckedBinary (node);
			else
				VisitSimpleBinary (node);

			return node;
		}

		void VisitSimpleBinary (BinaryExpression node)
		{
			Visit (node.Left);
			WriteSpace ();
			WriteToken (GetBinaryOperator (node.NodeType));
			WriteSpace ();
			Visit (node.Right);
		}

		void VisitCheckedBinary (BinaryExpression node)
		{
			WriteKeyword ("checked");
			WriteSpace ();
			WriteToken ("{");

			WriteSpace ();

			VisitSimpleBinary (node);

			WriteSpace ();

			WriteToken ("}");
		}

		static string GetBinaryOperator (ExpressionType type)
		{
			switch (type) {
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
				return "+";
			case ExpressionType.AddAssign:
			case ExpressionType.AddAssignChecked:
				return "+=";
			case ExpressionType.And:
				return "&";
			case ExpressionType.AndAssign:
				return "&=";
			case ExpressionType.Assign:
				return "=";
			case ExpressionType.Coalesce:
				return "??";
			case ExpressionType.Divide:
				return "/";
			case ExpressionType.DivideAssign:
				return "/=";
			case ExpressionType.ExclusiveOr:
				return "^";
			case ExpressionType.ExclusiveOrAssign:
				return "^=";
			case ExpressionType.GreaterThan:
				return ">";
			case ExpressionType.GreaterThanOrEqual:
				return ">=";
			case ExpressionType.LeftShift:
				return "<<";
			case ExpressionType.LeftShiftAssign:
				return "<<=";
			case ExpressionType.LessThan:
				return "<";
			case ExpressionType.LessThanOrEqual:
				return "<=";
			case ExpressionType.Modulo:
				return "%";
			case ExpressionType.ModuloAssign:
				return "%=";
			case ExpressionType.Multiply:
			case ExpressionType.MultiplyChecked:
				return "*";
			case ExpressionType.MultiplyAssign:
			case ExpressionType.MultiplyAssignChecked:
				return "*=";
			case ExpressionType.Or:
				return "|";
			case ExpressionType.OrAssign:
				return "|=";
			case ExpressionType.RightShift:
				return ">>";
			case ExpressionType.RightShiftAssign:
				return ">>=";
			case ExpressionType.Subtract:
			case ExpressionType.SubtractChecked:
				return "-";
			case ExpressionType.SubtractAssign:
			case ExpressionType.SubtractAssignChecked:
				return "-=";
			default:
				throw new NotImplementedException (type.ToString ());
			}
		}

		static bool IsChecked (ExpressionType type)
		{
			switch (type) {
			case ExpressionType.AddAssignChecked:
			case ExpressionType.AddChecked:
			case ExpressionType.ConvertChecked:
			case ExpressionType.MultiplyAssignChecked:
			case ExpressionType.MultiplyChecked:
			case ExpressionType.NegateChecked:
			case ExpressionType.SubtractAssignChecked:
			case ExpressionType.SubtractChecked:
				return true;
			default:
				return false;
			}
		}

		protected override Expression VisitParameter (ParameterExpression node)
		{
			WriteIdentifier (node.Name, node);

			return node;
		}

		protected override Expression VisitGoto (GotoExpression node)
		{
			switch (node.Kind) {
			case GotoExpressionKind.Return:
				WriteKeyword ("return");
				WriteSpace ();
				Visit (node.Value);
				break;
			default:
				throw new NotImplementedException ();
			}

			return node;
		}

		protected override Expression VisitConstant (ConstantExpression node)
		{
			WriteLiteral (node.Value == null ? "null" : node.Value.ToString ());

			return node;
		}

		protected override Expression VisitLabel (LabelExpression node)
		{
			return node;
		}

		protected override LabelTarget VisitLabelTarget (LabelTarget target)
		{
			Dedent ();
			WriteIdentifier (target.Name, target);
			WriteToken (":");
			WriteLine ();
			Indent ();

			return target;
		}
	}
}
