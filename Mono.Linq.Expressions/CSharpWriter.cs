//
// CSharpWriter.cs
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
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mono.Linq.Expressions {

	public class CSharpWriter : ExpressionWriter {

		readonly Dictionary<ParameterExpression, string> unique_names = new Dictionary<ParameterExpression, string> ();

		int unique_seed;

		public CSharpWriter (IFormatter formatter)
			: base (formatter)
		{
		}

		public override void Write (LambdaExpression expression)
		{
			VisitLambdaSignature (expression);
			VisitLambdaBody (expression);
		}

		protected override Expression VisitLambda<T> (Expression<T> node)
		{
			VisitParameters (node);
			WriteSpace ();
			WriteToken ("=>");
			WriteLine ();

			VisitLambdaBody (node);

			return node;
		}

		void VisitLambdaSignature (LambdaExpression node)
		{
			VisitType (node.ReturnType);
			WriteSpace ();
			WriteIdentifier (node.Name, node);
			VisitParameters (node);

			WriteLine ();
		}

		void VisitParameters (LambdaExpression node)
		{
			VisitParenthesizedList (node.Parameters, parameter => {
				VisitType (parameter.Type);
				WriteSpace ();
				WriteIdentifier (NameFor (parameter), parameter);
			});
		}

		string NameFor (ParameterExpression parameter)
		{
			if (!string.IsNullOrEmpty (parameter.Name))
				return parameter.Name;

			var name = GeneratedNameFor (parameter);
			if (name != null)
				return name;

			name = "var_$" + unique_seed++;
			unique_names.Add (parameter, name);
			return name;
		}

		string GeneratedNameFor (ParameterExpression parameter)
		{
			string name;
			if (!unique_names.TryGetValue (parameter, out name))
				return null;

			return name;
		}

		void VisitLambdaBody (LambdaExpression node)
		{
			if (node.Body.NodeType != ExpressionType.Block)
				VisitSingleExpressionBody (node);
			else
				VisitBlockExpressionBody (node);
		}

		void VisitBlockExpressionBody (LambdaExpression node)
		{
			VisitBlockExpression ((BlockExpression) node.Body);
		}

		static bool IsStatement (Expression expression)
		{
			switch (expression.NodeType) {
			case ExpressionType.Conditional:
				return !IsTernaryConditional ((ConditionalExpression) expression);
			case ExpressionType.Try:
			case ExpressionType.Loop:
			case ExpressionType.Switch:
				return true;
			default:
				var custom = expression as CustomExpression;
				if (custom != null)
					return IsStatement (custom);

				return false;
			}
		}

		static bool IsStatement (CustomExpression expression)
		{
			switch (expression.CustomNodeType) {
			case CustomExpressionType.DoWhileExpression:
			case CustomExpressionType.ForExpression:
			case CustomExpressionType.ForEachExpression:
			case CustomExpressionType.UsingExpression:
			case CustomExpressionType.WhileExpression:
				return true;
			default:
				return false;
			}
		}

		static bool IsActualStatement (Expression expression)
		{
			switch (expression.NodeType) {
			case ExpressionType.Label:
				return false;
			case ExpressionType.Conditional:
				return IsTernaryConditional ((ConditionalExpression) expression);
			case ExpressionType.Try:
			case ExpressionType.Loop:
			case ExpressionType.Switch:
				return false;
			default:
				return true;
			}
		}


		void VisitSingleExpressionBody (LambdaExpression node)
		{
			VisitBlock (() => {
				if (node.ReturnType != typeof (void) && !IsStatement (node.Body)) {
					WriteKeyword ("return");
					WriteSpace ();
				}

				Visit (node.Body);

				if (!IsStatement (node.Body)) {
					WriteToken (";");
					WriteLine ();
				}
			});
		}

		void VisitType (Type type)
		{
			if (type.IsArray) {
				VisitArrayType (type);
				return;
			}

			if (type.IsGenericParameter) {
				WriteReference (type.Name, type);
				return;
			}

			if (type.IsGenericType && type.IsGenericTypeDefinition) {
				VisitGenericTypeDefinition (type);
				return;
			}

			if (type.IsGenericType && !type.IsGenericTypeDefinition) {
				VisitGenericTypeInstance (type);
				return;
			}

			VisitSimpleType (type);
		}

		void VisitArrayType (Type type)
		{
			VisitType (type.GetElementType ());
			WriteToken ("[");
			for (int i = 1; i < type.GetArrayRank (); i++)
				WriteToken (",");
			WriteToken ("]");
		}

		void VisitGenericTypeDefinition (Type type)
		{
			WriteReference (CleanGenericName (type), type);
			WriteToken ("<");
			var arity = type.GetGenericArguments ().Length;
			for (int i = 1; i < arity; i++)
				WriteToken (",");
			WriteToken (">");
		}

		void VisitGenericTypeInstance (Type type)
		{
			WriteReference (CleanGenericName (type), type);

			VisitGenericArguments (type.GetGenericArguments ());
		}

		void VisitGenericArguments (Type [] generic_arguments)
		{
			VisitList (generic_arguments, "<", VisitType, ">");
		}

		static string CleanGenericName (Type type)
		{
			var name = type.Name;
			var position = name.LastIndexOf ("`");
			if (position == -1)
				return name;

			return name.Substring (0, position);
		}

		void VisitSimpleType (Type type)
		{
			WriteReference (GetSimpleTypeName (type), type);
		}

		static string GetSimpleTypeName (Type type)
		{
			if (type == typeof (void))
				return "void";
			if (type == typeof (object))
				return "object";

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
				VisitType (variable.Type);
				WriteSpace ();
				WriteIdentifier (NameFor (variable), variable);
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
			if (last.Is (ExpressionType.Goto) && ((GotoExpression) last).Kind == GotoExpressionKind.Return)
				return false;

			return true;
		}

		protected override Expression VisitBinary (BinaryExpression node)
		{
			if (IsChecked (node.NodeType))
				VisitCheckedBinary (node);
			else if (node.Is (ExpressionType.Assign))
				VisitAssign (node);
			else if (IsPower (node.NodeType))
				VisitPower (node);
			else if (node.Is (ExpressionType.ArrayIndex))
				VisitArrayIndex (node);
			else
				VisitSimpleBinary (node);

			return node;
		}

		void VisitArrayIndex (BinaryExpression node)
		{
			Visit (node.Left);
			WriteToken ("[");
			Visit (node.Right);
			WriteToken ("]");
		}

		void VisitAssign (BinaryExpression node)
		{
			Visit (node.Left);
			WriteSpace ();
			WriteToken (GetBinaryOperator (node.NodeType));
			WriteSpace ();
			Visit (node.Right);
		}

		void VisitPower (BinaryExpression node)
		{
			var pow = Expression.Call (typeof (System.Math).GetMethod ("Pow"), node.Left, node.Right);

			if (node.Is (ExpressionType.Power))
				Visit (pow);
			else if (node.Is (ExpressionType.PowerAssign))
				Visit (Expression.Assign (node.Left, pow));
		}

		static bool IsPower (ExpressionType type)
		{
			return type == ExpressionType.Power || type == ExpressionType.PowerAssign;
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
			VisitChecked (() => VisitSimpleBinary (node));
		}

		void VisitChecked (Action action)
		{
			WriteKeyword ("checked");
			WriteSpace ();
			WriteToken ("{");

			WriteSpace ();

			action ();

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

		protected override Expression VisitUnary (UnaryExpression node)
		{
			if (IsChecked (node.NodeType)) {
				VisitCheckedUnary (node);
				return node;
			}

			switch (node.NodeType) {
			case ExpressionType.Throw:
				VisitThrow (node);
				break;
			case ExpressionType.IsTrue:
				VisitIsTrue (node);
				break;
			case ExpressionType.IsFalse:
				VisitIsFalse (node);
				break;
			case ExpressionType.ArrayLength:
				VisitArrayLength (node);
				break;
			case ExpressionType.TypeAs:
				VisitTypeAs (node);
				break;
			case ExpressionType.Increment:
				VisitIncrement (node);
				break;
			case ExpressionType.Decrement:
				VisitDecrement (node);
				break;
			case ExpressionType.PreDecrementAssign:
				VisitPreDecrementAssign (node);
				break;
			case ExpressionType.PostDecrementAssign:
				VisitPostDecrementAssign (node);
				break;
			case ExpressionType.PreIncrementAssign:
				VisitPreIncrementAssign (node);
				break;
			case ExpressionType.PostIncrementAssign:
				VisitPostIncrementAssign (node);
				break;
			case ExpressionType.ConvertChecked:
				VisitConvertChecked (node);
				break;
			case ExpressionType.Convert:
			case ExpressionType.Unbox:
				VisitConvert (node);
				break;
			case ExpressionType.Quote:
				Visit (node.Operand);
				break;
			default:
				VisitSimpleUnary (node);
				break;
			}

			return node;
		}

		void VisitConvert (UnaryExpression node)
		{
			WriteToken ("(");
			VisitType (node.Type);
			WriteToken (")");

			VisitParenthesizedExpression (node.Operand);
		}

		void VisitConvertChecked (UnaryExpression node)
		{
			VisitChecked (() => VisitConvert (node));
		}

		void VisitPostIncrementAssign (UnaryExpression node)
		{
			Visit (node.Operand);
			WriteToken ("++");
		}

		void VisitPreIncrementAssign (UnaryExpression node)
		{
			WriteToken ("++");
			Visit (node.Operand);
		}

		void VisitPostDecrementAssign (UnaryExpression node)
		{
			Visit (node.Operand);
			WriteToken ("--");
		}

		void VisitPreDecrementAssign (UnaryExpression node)
		{
			WriteToken ("--");
			Visit (node.Operand);
		}

		void VisitDecrement (UnaryExpression node)
		{
			Visit (Expression.Subtract (node.Operand, Expression.Constant (1)));
		}

		void VisitIncrement (UnaryExpression node)
		{
			Visit (Expression.Add (node.Operand, Expression.Constant (1)));
		}

		void VisitIsTrue (UnaryExpression node)
		{
			Visit (Expression.Equal (node.Operand, Expression.Constant (true)));
		}

		void VisitIsFalse (UnaryExpression node)
		{
			Visit (Expression.Equal (node.Operand, Expression.Constant (false)));
		}

		void VisitArrayLength (UnaryExpression node)
		{
			Visit (Expression.Property (node.Operand, "Length"));
		}

		void VisitTypeAs (UnaryExpression node)
		{
			Visit (node.Operand);
			WriteSpace ();
			WriteKeyword ("as");
			WriteSpace ();
			VisitType (node.Type);
		}

		void VisitThrow (UnaryExpression node)
		{
			WriteKeyword ("throw");
			WriteSpace ();
			Visit (node.Operand);
		}

		void VisitCheckedUnary (UnaryExpression node)
		{
			VisitChecked (() => VisitSimpleUnary (node));
		}

		void VisitSimpleUnary (UnaryExpression node)
		{
			WriteToken (GetUnaryOperator (node.NodeType));
			VisitParenthesizedExpression (node.Operand);
		}

		static string GetUnaryOperator (ExpressionType type)
		{
			switch (type) {
			case ExpressionType.UnaryPlus:
				return "+";
			case ExpressionType.Not:
				return "!";
			case ExpressionType.Negate:
			case ExpressionType.NegateChecked:
				return "-";
			case ExpressionType.OnesComplement:
				return "~";
			default:
				throw new NotImplementedException (type.ToString ());
			}
		}

		protected override Expression VisitParameter (ParameterExpression node)
		{
			WriteIdentifier (NameFor (node), node);

			return node;
		}

		protected override Expression VisitConditional (ConditionalExpression node)
		{
			if (IsTernaryConditional (node))
				VisitConditionalExpression (node);
			else
				VisitConditionalStatement (node);

			return node;
		}

		void VisitConditionalExpression (ConditionalExpression node)
		{
			Visit (node.Test);
			WriteSpace ();
			WriteToken ("?");
			WriteSpace ();
			Visit (node.IfTrue);
			WriteSpace ();
			WriteToken (":");
			WriteSpace ();
			Visit (node.IfFalse);
		}

		void VisitConditionalStatement (ConditionalExpression node)
		{
			WriteKeyword ("if");
			WriteSpace ();
			WriteToken ("(");

			Visit (node.Test);

			WriteToken (")");
			WriteLine ();

			VisitAsBlock (node.IfTrue);

			if (node.IfFalse != null) {
				WriteKeyword ("else");
				WriteLine ();

				VisitAsBlock (node.IfFalse);
			}
		}

		static bool IsTernaryConditional (ConditionalExpression node)
		{
			return node.Type != typeof (void) && (node.IfTrue.NodeType != ExpressionType.Block
				|| (node.IfFalse != null && node.IfFalse.NodeType != ExpressionType.Block));
		}

		protected override Expression VisitGoto (GotoExpression node)
		{
			switch (node.Kind) {
			case GotoExpressionKind.Return:
				WriteKeyword ("return");
				WriteSpace ();
				Visit (node.Value);
				break;
			case GotoExpressionKind.Break:
				WriteKeyword ("break");
				break;
			case GotoExpressionKind.Continue:
				WriteKeyword ("continue");
				break;
			case GotoExpressionKind.Goto:
				WriteKeyword ("goto");
				WriteSpace ();
				Visit (node.Value);
				break;
			default:
				throw new NotSupportedException ();
			}

			return node;
		}

		protected override Expression VisitConstant (ConstantExpression node)
		{
			WriteLiteral (GetLiteral (node.Value));

			return node;
		}

		static string GetLiteral (object value)
		{
			if (value == null)
				return "null";

			switch (Type.GetTypeCode (value.GetType ())) {
			case TypeCode.Boolean:
				return ((bool) value) ? "true" : "false";
			case TypeCode.Char:
				return "'" + ((char) value) + "'";
			case TypeCode.String:
				return "\"" + ((string) value) + "\"";
			case TypeCode.Int32:
				return ((IFormattable) value).ToString (null, System.Globalization.CultureInfo.InvariantCulture);
			default:
				return value.ToString ();
			}
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

		protected override Expression VisitInvocation (InvocationExpression node)
		{
			Visit (node.Expression);
			VisitArguments (node.Arguments);

			return node;
		}

		protected override Expression VisitMethodCall (MethodCallExpression node)
		{
			var method = node.Method;

			if (node.Object != null)
				Visit (node.Object);
			else
				VisitType (method.DeclaringType);

			WriteToken (".");

			WriteReference (method.Name, method);

			if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
				VisitGenericArguments (method.GetGenericArguments ());

			VisitArguments (node.Arguments);

			return node;
		}

		void VisitParenthesizedList<T> (IList<T> list, Action<T> writer)
		{
			VisitList (list, "(", writer, ")");
		}

		void VisitBracedList<T> (IList<T> list, Action<T> writer)
		{
			VisitList (list, "{", writer, "}");
		}

		void VisitBracketedList<T> (IList<T> list, Action<T> writer)
		{
			VisitList (list, "[", writer, "]");
		}

		void VisitList<T> (IList<T> list, string opening, Action<T> writer, string closing)
		{
			WriteToken (opening);

			for (int i = 0; i < list.Count; i++) {
				if (i > 0) {
					WriteToken (",");
					WriteSpace ();
				}

				writer (list [i]);
			}

			WriteToken (closing);
		}

		void VisitArguments (IList<Expression> expressions)
		{
			VisitParenthesizedList (expressions, e => Visit (e));
		}

		protected override Expression VisitNew (NewExpression node)
		{
			WriteKeyword ("new");
			WriteSpace ();
			VisitType (node.Constructor.DeclaringType);
			VisitArguments (node.Arguments);

			return node;
		}

		protected override Expression VisitMember (MemberExpression node)
		{
			if (node.Expression != null)
				Visit (node.Expression);
			else
				VisitType (node.Member.DeclaringType);

			WriteToken (".");
			WriteReference (node.Member.Name, node.Member);

			return node;
		}

		protected override Expression VisitIndex (IndexExpression node)
		{
			Visit (node.Object);
			VisitBracketedList (node.Arguments, expression => Visit (expression));

			return node;
		}

		protected override Expression VisitNewArray (NewArrayExpression node)
		{
			if (node.Is (ExpressionType.NewArrayInit))
				VisitNewArrayInit (node);
			else if (node.Is (ExpressionType.NewArrayBounds))
				VisitNewArrayBounds (node);

			return node;
		}

		void VisitNewArrayBounds (NewArrayExpression node)
		{
			WriteKeyword ("new");
			WriteSpace ();
			VisitType (node.Type.GetElementType ());

			VisitBracketedList (node.Expressions, expression => Visit (expression));
		}

		void VisitNewArrayInit (NewArrayExpression node)
		{
			WriteKeyword ("new");
			WriteSpace ();
			VisitType (node.Type);
			WriteSpace ();

			VisitBracedList (node.Expressions, expression => Visit (expression));
		}

		protected override Expression VisitListInit (ListInitExpression node)
		{
			Visit (node.NewExpression);
			WriteSpace ();

			VisitInitializers (node.Initializers);

			return node;
		}

		void VisitInitializers (IList<ElementInit> initializers)
		{
			VisitBracedList (initializers, initializer => VisitElementInit (initializer));
		}

		protected override Expression VisitMemberInit (MemberInitExpression node)
		{
			Visit (node.NewExpression);

			VisitBindings (node.Bindings);

			return node;
		}

		void VisitBindings (IList<MemberBinding> bindings)
		{
			WriteLine ();

			WriteToken ("{");
			WriteLine ();
			Indent ();

			for (int i = 0; i < bindings.Count; i++) {
				var binding = bindings [i];

				VisitMemberBinding (binding);

				if (i < bindings.Count - 1)
					WriteToken (",");

				WriteLine ();
			}

			Dedent ();
			WriteToken ("}");
		}

		protected override MemberAssignment VisitMemberAssignment (MemberAssignment node)
		{
			VisitMemberBindingMember (node);
			WriteSpace ();

			Visit (node.Expression);

			return node;
		}

		void VisitMemberBindingMember (MemberBinding node)
		{
			WriteReference (node.Member.Name, node.Member);
			WriteSpace ();
			WriteToken ("=");
		}

		protected override MemberListBinding VisitMemberListBinding (MemberListBinding node)
		{
			VisitMemberBindingMember (node);
			WriteSpace ();

			VisitInitializers (node.Initializers);

			return node;
		}

		protected override MemberMemberBinding VisitMemberMemberBinding (MemberMemberBinding node)
		{
			VisitMemberBindingMember (node);

			VisitBindings (node.Bindings);

			return node;
		}

		protected override ElementInit VisitElementInit (ElementInit node)
		{
			if (node.Arguments.Count == 1)
				Visit (node.Arguments [0]);
			else
				VisitBracedList (node.Arguments, expression => Visit (expression));

			return node;
		}

		protected override Expression VisitTypeBinary (TypeBinaryExpression node)
		{
			if (node.Is (ExpressionType.TypeEqual))
				VisitTypeEqual (node);
			else if (node.Is (ExpressionType.TypeIs))
				VisitTypeIs (node);

			return node;
		}

		void VisitTypeIs (TypeBinaryExpression node)
		{
			Visit (node.Expression);
			WriteSpace ();
			WriteKeyword ("is");
			WriteSpace ();
			VisitType (node.TypeOperand);
		}

		void VisitTypeEqual (TypeBinaryExpression node)
		{
			Visit (Expression.Call (
				node.Expression,
				typeof (object).GetMethod ("GetType", Type.EmptyTypes)));

			WriteSpace ();
			WriteToken ("==");
			WriteSpace ();

			WriteKeyword ("typeof");
			WriteToken ("(");
			VisitType (node.TypeOperand);
			WriteToken (")");
		}

		protected override Expression VisitTry (TryExpression node)
		{
			WriteKeyword ("try");
			WriteLine ();
			VisitAsBlock (node.Body);

			foreach (var handler in node.Handlers)
				VisitCatchBlock (handler);

			if (node.Fault != null) {
				WriteKeyword ("fault");
				WriteLine ();
				VisitAsBlock (node.Fault);
			}

			if (node.Finally != null) {
				WriteKeyword ("finally");
				WriteLine ();
				VisitAsBlock (node.Finally);
			}

			return node;
		}

		void VisitAsBlock (Expression node)
		{
			Visit (node.Is (ExpressionType.Block) ? node : Expression.Block (node));
			WriteLine ();
		}

		protected override CatchBlock VisitCatchBlock (CatchBlock node)
		{
			WriteKeyword ("catch");

			WriteSpace ();
			WriteToken ("(");
			VisitType (node.Test);
			if (node.Variable != null) {
				WriteSpace ();
				WriteIdentifier (NameFor (node.Variable), node.Variable);
			}
			WriteToken (")");

			if (node.Filter != null) {
				WriteSpace ();
				WriteKeyword ("if");
				WriteSpace ();
				WriteToken ("(");
				Visit (node.Filter);
				WriteToken (")");
			}
			WriteLine ();

			VisitAsBlock (node.Body);

			return node;
		}

		protected override Expression VisitLoop (LoopExpression node)
		{
			WriteKeyword ("for");
			WriteSpace ();
			WriteToken ("(");
			WriteToken (";");
			WriteToken (";");
			WriteToken (")");
			WriteLine ();

			VisitAsBlock (node.Body);

			return node;
		}

		protected override Expression VisitSwitch (SwitchExpression node)
		{
			WriteKeyword ("switch");
			WriteSpace ();
			WriteToken ("(");
			Visit (node.SwitchValue);
			WriteToken (")");
			WriteLine ();

			VisitBlock (() => {
				foreach (var @case in node.Cases)
					VisitSwitchCase (@case);

				if (node.DefaultBody != null) {
					WriteKeyword ("default");
					WriteToken (":");
					WriteLine ();

					VisitAsBlock (node.DefaultBody);
				}
			});

			WriteLine ();

			return node;
		}

		protected override SwitchCase VisitSwitchCase (SwitchCase node)
		{
			foreach (var value in node.TestValues) {
				WriteKeyword ("case");
				WriteSpace ();
				Visit (value);
				WriteToken (":");
				WriteLine ();
			}

			VisitAsBlock (node.Body);

			return node;
		}

		protected override Expression VisitDefault (DefaultExpression node)
		{
			WriteKeyword ("default");
			WriteToken ("(");
			VisitType (node.Type);
			WriteToken (")");

			return node;
		}

		protected internal override Expression VisitForExpression (ForExpression node)
		{
			WriteKeyword ("for");
			WriteSpace ();
			WriteToken ("(");
			VisitType (node.Variable.Type);
			WriteSpace ();
			Visit (node.Variable);
			WriteSpace ();
			WriteToken ("=");
			WriteSpace ();
			Visit (node.Initializer);
			WriteToken (";");
			WriteSpace ();
			Visit (node.Test);
			WriteToken (";");
			WriteSpace ();
			Visit (node.Step);
			WriteToken (")");
			WriteLine ();

			VisitAsBlock (node.Body);

			return node;
		}

		protected internal override Expression VisitForEachExpression (ForEachExpression node)
		{
			WriteKeyword ("foreach");
			WriteSpace ();
			WriteToken ("(");
			VisitType (node.Variable.Type);
			WriteSpace ();
			Visit (node.Variable);
			WriteSpace ();
			WriteKeyword ("in");
			WriteSpace ();
			Visit (node.Enumerable);
			WriteToken (")");
			WriteLine ();

			VisitAsBlock (node.Body);

			return node;
		}

		protected internal override Expression VisitUsingExpression (UsingExpression node)
		{
			WriteKeyword ("using");
			WriteSpace ();
			WriteToken ("(");
			Visit (node.Disposable);
			WriteToken (")");
			WriteLine ();

			VisitAsBlock (node.Body);

			return node;
		}

		protected internal override Expression VisitDoWhileExpression (DoWhileExpression node)
		{
			WriteKeyword ("do");
			WriteLine ();

			VisitAsBlock (node.Body);

			WriteKeyword ("while");
			WriteSpace ();
			WriteToken ("(");
			Visit (node.Test);
			WriteToken (")");
			WriteToken (";");
			WriteLine ();

			return node;
		}

		protected internal override Expression VisitWhileExpression (WhileExpression node)
		{
			WriteKeyword ("while");
			WriteSpace ();
			WriteToken ("(");
			Visit (node.Test);
			WriteToken (")");
			WriteLine ();

			VisitAsBlock (node.Body);

			return node;
		}
	}
}
