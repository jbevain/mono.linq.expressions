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
			VisitBlockExpression ((BlockExpression) node.Body);
		}

		static bool IsNotStatement (Expression expression)
		{
			switch (expression.NodeType) {
			case ExpressionType.Conditional:
				return IsTernaryConditional ((ConditionalExpression) expression);
			default:
				return true;
			}
		}

		void VisitSingleExpressionBody<T> (Expression<T> node)
		{
			VisitBlock (() => {
				if (node.ReturnType != typeof (void) && IsNotStatement (node.Body)) {
					WriteKeyword ("return");
					WriteSpace ();
				}

				Visit (node.Body);

				if (IsNotStatement (node.Body)) {
					WriteToken (";");
					WriteLine ();
				}
			});
		}

		void WriteType (Type type)
		{
			WriteReference (GetTypeName (type), type);
		}

		static string GetTypeName (Type type)
		{
			if (type == typeof (void))
				return "void";

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
			VisitBlockExpression (node);

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

		void VisitBlockExpression (BlockExpression node)
		{
			VisitBlock (() => {
				VisitBlockVariables (node);
				VisitBlockExpressions (node);
			});
		}

		void VisitBlockExpressions (BlockExpression node)
		{
			for (int i = 0; i < node.Expressions.Count; i++) {
				var expression = node.Expressions [i];

				if (IsActualStatement (expression) && RequiresExplicitReturn (node, i, node.Type != typeof (void))) {
					WriteKeyword ("return");
					WriteSpace ();
				}

				Write (expression);

				if (!IsActualStatement (expression))
					continue;

				WriteToken (";");
				WriteLine ();
			}
		}

		void VisitBlockVariables (BlockExpression node)
		{
			foreach (var variable in node.Variables) {
				WriteType (variable.Type);
				WriteSpace ();
				WriteIdentifier (variable.Name, variable);
				WriteToken (";");
				WriteLine ();
			}

			if (node.Variables.Count > 0)
				WriteLine ();
		}

		static bool RequiresExplicitReturn (BlockExpression node, int index, bool return_last)
		{
			if (!return_last)
				return false;

			var last_index = node.Expressions.Count - 1;
			if (index != last_index)
				return false;

			var last = node.Expressions [last_index];
			if (last.NodeType == ExpressionType.Goto && ((GotoExpression) last).Kind == GotoExpressionKind.Return)
				return false;

			return true;
		}

		static bool IsActualStatement (Expression expression)
		{
			switch (expression.NodeType) {
			case ExpressionType.Label:
				return false;
			case ExpressionType.Conditional:
				return IsTernaryConditional ((ConditionalExpression) expression);
			default:
				return true;
			}
		}

		protected override Expression VisitBinary (BinaryExpression node)
		{
			if (IsChecked (node.NodeType))
				VisitCheckedBinary (node);
			else if (node.NodeType == ExpressionType.Assign) {
				Visit (node.Left);
				WriteSpace ();
				WriteToken (GetBinaryOperator (node.NodeType));
				WriteSpace ();
				Visit (node.Right);
			} else
				VisitSimpleBinary (node);

			return node;
		}

		void VisitSimpleBinary (BinaryExpression node)
		{
			VisitParenthesizedExpression (node.Left);
			WriteSpace ();
			WriteToken (GetBinaryOperator (node.NodeType));
			WriteSpace ();
			VisitParenthesizedExpression (node.Right);
		}

		void VisitParenthesizedExpression (Expression expression)
		{
			if (RequiresParentheses (expression)) {
				WriteToken ("(");
				Visit (expression);
				WriteToken (")");
				return;
			}

			Visit (expression);
		}

		static bool RequiresParentheses (Expression expression)
		{
			switch (expression.NodeType) {
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
			case ExpressionType.And:
			case ExpressionType.AndAlso:
			case ExpressionType.Coalesce:
			case ExpressionType.Decrement:
			case ExpressionType.Divide:
			case ExpressionType.Equal:
			case ExpressionType.ExclusiveOr:
			case ExpressionType.GreaterThan:
			case ExpressionType.GreaterThanOrEqual:
			case ExpressionType.Increment:
			case ExpressionType.LeftShift:
			case ExpressionType.LessThan:
			case ExpressionType.LessThanOrEqual:
			case ExpressionType.Modulo:
			case ExpressionType.Multiply:
			case ExpressionType.MultiplyChecked:
			case ExpressionType.Negate:
			case ExpressionType.Not:
			case ExpressionType.NotEqual:
			case ExpressionType.OnesComplement:
			case ExpressionType.Or:
			case ExpressionType.OrElse:
			case ExpressionType.Power:
			case ExpressionType.RightShift:
			case ExpressionType.Subtract:
			case ExpressionType.SubtractChecked:
			case ExpressionType.UnaryPlus:
				return true;
			default:
				return false;
			}
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
			case ExpressionType.AndAlso:
				return "&&";
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
			case ExpressionType.Equal:
				return "==";
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
			case ExpressionType.NotEqual:
				return "!=";
			case ExpressionType.Or:
				return "|";
			case ExpressionType.OrAssign:
				return "|=";
			case ExpressionType.OrElse:
				return "||";
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

		protected override Expression VisitConditional (ConditionalExpression node)
		{
			if (IsTernaryConditional (node))
				throw new NotImplementedException ();

			WriteKeyword ("if");
			WriteSpace ();
			WriteToken ("(");

			Visit (node.Test);

			WriteToken (")");
			WriteLine ();

			Visit (node.IfTrue);

			if (node.IfFalse != null) {
				WriteKeyword ("else");
				WriteLine ();

				Visit (node.IfFalse);
			}

			return node;
		}

		static bool IsTernaryConditional (ConditionalExpression node)
		{
			return node.IfTrue.NodeType != ExpressionType.Block
				|| (node.IfFalse != null && node.IfFalse.NodeType != ExpressionType.Block);
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
