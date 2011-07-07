//
// FluentExtensions.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2011 Novell, Inc. (http://www.novell.com)
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

// generated

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Mono.Linq.Expressions {

	public static class FluentExtensions {

		public static BinaryExpression Assign (this Expression left, Expression right) {
			return Expression.Assign (left, right);
		}

		public static BinaryExpression Equal (this Expression left, Expression right) {
			return Expression.Equal (left, right);
		}

		public static BinaryExpression Equal (this Expression left, Expression right, Boolean liftToNull, MethodInfo method) {
			return Expression.Equal (left, right, liftToNull, method);
		}

		public static BinaryExpression ReferenceEqual (this Expression left, Expression right) {
			return Expression.ReferenceEqual (left, right);
		}

		public static BinaryExpression NotEqual (this Expression left, Expression right) {
			return Expression.NotEqual (left, right);
		}

		public static BinaryExpression NotEqual (this Expression left, Expression right, Boolean liftToNull, MethodInfo method) {
			return Expression.NotEqual (left, right, liftToNull, method);
		}

		public static BinaryExpression ReferenceNotEqual (this Expression left, Expression right) {
			return Expression.ReferenceNotEqual (left, right);
		}

		public static BinaryExpression GreaterThan (this Expression left, Expression right) {
			return Expression.GreaterThan (left, right);
		}

		public static BinaryExpression GreaterThan (this Expression left, Expression right, Boolean liftToNull, MethodInfo method) {
			return Expression.GreaterThan (left, right, liftToNull, method);
		}

		public static BinaryExpression LessThan (this Expression left, Expression right) {
			return Expression.LessThan (left, right);
		}

		public static BinaryExpression LessThan (this Expression left, Expression right, Boolean liftToNull, MethodInfo method) {
			return Expression.LessThan (left, right, liftToNull, method);
		}

		public static BinaryExpression GreaterThanOrEqual (this Expression left, Expression right) {
			return Expression.GreaterThanOrEqual (left, right);
		}

		public static BinaryExpression GreaterThanOrEqual (this Expression left, Expression right, Boolean liftToNull, MethodInfo method) {
			return Expression.GreaterThanOrEqual (left, right, liftToNull, method);
		}

		public static BinaryExpression LessThanOrEqual (this Expression left, Expression right) {
			return Expression.LessThanOrEqual (left, right);
		}

		public static BinaryExpression LessThanOrEqual (this Expression left, Expression right, Boolean liftToNull, MethodInfo method) {
			return Expression.LessThanOrEqual (left, right, liftToNull, method);
		}

		public static BinaryExpression AndAlso (this Expression left, Expression right) {
			return Expression.AndAlso (left, right);
		}

		public static BinaryExpression AndAlso (this Expression left, Expression right, MethodInfo method) {
			return Expression.AndAlso (left, right, method);
		}

		public static BinaryExpression OrElse (this Expression left, Expression right) {
			return Expression.OrElse (left, right);
		}

		public static BinaryExpression OrElse (this Expression left, Expression right, MethodInfo method) {
			return Expression.OrElse (left, right, method);
		}

		public static BinaryExpression Coalesce (this Expression left, Expression right) {
			return Expression.Coalesce (left, right);
		}

		public static BinaryExpression Coalesce (this Expression left, Expression right, LambdaExpression conversion) {
			return Expression.Coalesce (left, right, conversion);
		}

		public static BinaryExpression Add (this Expression left, Expression right) {
			return Expression.Add (left, right);
		}

		public static BinaryExpression Add (this Expression left, Expression right, MethodInfo method) {
			return Expression.Add (left, right, method);
		}

		public static BinaryExpression AddAssign (this Expression left, Expression right) {
			return Expression.AddAssign (left, right);
		}

		public static BinaryExpression AddAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.AddAssign (left, right, method);
		}

		public static BinaryExpression AddAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.AddAssign (left, right, method, conversion);
		}

		public static BinaryExpression AddAssignChecked (this Expression left, Expression right) {
			return Expression.AddAssignChecked (left, right);
		}

		public static BinaryExpression AddAssignChecked (this Expression left, Expression right, MethodInfo method) {
			return Expression.AddAssignChecked (left, right, method);
		}

		public static BinaryExpression AddAssignChecked (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.AddAssignChecked (left, right, method, conversion);
		}

		public static BinaryExpression AddChecked (this Expression left, Expression right) {
			return Expression.AddChecked (left, right);
		}

		public static BinaryExpression AddChecked (this Expression left, Expression right, MethodInfo method) {
			return Expression.AddChecked (left, right, method);
		}

		public static BinaryExpression Subtract (this Expression left, Expression right) {
			return Expression.Subtract (left, right);
		}

		public static BinaryExpression Subtract (this Expression left, Expression right, MethodInfo method) {
			return Expression.Subtract (left, right, method);
		}

		public static BinaryExpression SubtractAssign (this Expression left, Expression right) {
			return Expression.SubtractAssign (left, right);
		}

		public static BinaryExpression SubtractAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.SubtractAssign (left, right, method);
		}

		public static BinaryExpression SubtractAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.SubtractAssign (left, right, method, conversion);
		}

		public static BinaryExpression SubtractAssignChecked (this Expression left, Expression right) {
			return Expression.SubtractAssignChecked (left, right);
		}

		public static BinaryExpression SubtractAssignChecked (this Expression left, Expression right, MethodInfo method) {
			return Expression.SubtractAssignChecked (left, right, method);
		}

		public static BinaryExpression SubtractAssignChecked (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.SubtractAssignChecked (left, right, method, conversion);
		}

		public static BinaryExpression SubtractChecked (this Expression left, Expression right) {
			return Expression.SubtractChecked (left, right);
		}

		public static BinaryExpression SubtractChecked (this Expression left, Expression right, MethodInfo method) {
			return Expression.SubtractChecked (left, right, method);
		}

		public static BinaryExpression Divide (this Expression left, Expression right) {
			return Expression.Divide (left, right);
		}

		public static BinaryExpression Divide (this Expression left, Expression right, MethodInfo method) {
			return Expression.Divide (left, right, method);
		}

		public static BinaryExpression DivideAssign (this Expression left, Expression right) {
			return Expression.DivideAssign (left, right);
		}

		public static BinaryExpression DivideAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.DivideAssign (left, right, method);
		}

		public static BinaryExpression DivideAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.DivideAssign (left, right, method, conversion);
		}

		public static BinaryExpression Modulo (this Expression left, Expression right) {
			return Expression.Modulo (left, right);
		}

		public static BinaryExpression Modulo (this Expression left, Expression right, MethodInfo method) {
			return Expression.Modulo (left, right, method);
		}

		public static BinaryExpression ModuloAssign (this Expression left, Expression right) {
			return Expression.ModuloAssign (left, right);
		}

		public static BinaryExpression ModuloAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.ModuloAssign (left, right, method);
		}

		public static BinaryExpression ModuloAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.ModuloAssign (left, right, method, conversion);
		}

		public static BinaryExpression Multiply (this Expression left, Expression right) {
			return Expression.Multiply (left, right);
		}

		public static BinaryExpression Multiply (this Expression left, Expression right, MethodInfo method) {
			return Expression.Multiply (left, right, method);
		}

		public static BinaryExpression MultiplyAssign (this Expression left, Expression right) {
			return Expression.MultiplyAssign (left, right);
		}

		public static BinaryExpression MultiplyAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.MultiplyAssign (left, right, method);
		}

		public static BinaryExpression MultiplyAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.MultiplyAssign (left, right, method, conversion);
		}

		public static BinaryExpression MultiplyAssignChecked (this Expression left, Expression right) {
			return Expression.MultiplyAssignChecked (left, right);
		}

		public static BinaryExpression MultiplyAssignChecked (this Expression left, Expression right, MethodInfo method) {
			return Expression.MultiplyAssignChecked (left, right, method);
		}

		public static BinaryExpression MultiplyAssignChecked (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.MultiplyAssignChecked (left, right, method, conversion);
		}

		public static BinaryExpression MultiplyChecked (this Expression left, Expression right) {
			return Expression.MultiplyChecked (left, right);
		}

		public static BinaryExpression MultiplyChecked (this Expression left, Expression right, MethodInfo method) {
			return Expression.MultiplyChecked (left, right, method);
		}

		public static BinaryExpression LeftShift (this Expression left, Expression right) {
			return Expression.LeftShift (left, right);
		}

		public static BinaryExpression LeftShift (this Expression left, Expression right, MethodInfo method) {
			return Expression.LeftShift (left, right, method);
		}

		public static BinaryExpression LeftShiftAssign (this Expression left, Expression right) {
			return Expression.LeftShiftAssign (left, right);
		}

		public static BinaryExpression LeftShiftAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.LeftShiftAssign (left, right, method);
		}

		public static BinaryExpression LeftShiftAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.LeftShiftAssign (left, right, method, conversion);
		}

		public static BinaryExpression RightShift (this Expression left, Expression right) {
			return Expression.RightShift (left, right);
		}

		public static BinaryExpression RightShift (this Expression left, Expression right, MethodInfo method) {
			return Expression.RightShift (left, right, method);
		}

		public static BinaryExpression RightShiftAssign (this Expression left, Expression right) {
			return Expression.RightShiftAssign (left, right);
		}

		public static BinaryExpression RightShiftAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.RightShiftAssign (left, right, method);
		}

		public static BinaryExpression RightShiftAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.RightShiftAssign (left, right, method, conversion);
		}

		public static BinaryExpression And (this Expression left, Expression right) {
			return Expression.And (left, right);
		}

		public static BinaryExpression And (this Expression left, Expression right, MethodInfo method) {
			return Expression.And (left, right, method);
		}

		public static BinaryExpression AndAssign (this Expression left, Expression right) {
			return Expression.AndAssign (left, right);
		}

		public static BinaryExpression AndAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.AndAssign (left, right, method);
		}

		public static BinaryExpression AndAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.AndAssign (left, right, method, conversion);
		}

		public static BinaryExpression Or (this Expression left, Expression right) {
			return Expression.Or (left, right);
		}

		public static BinaryExpression Or (this Expression left, Expression right, MethodInfo method) {
			return Expression.Or (left, right, method);
		}

		public static BinaryExpression OrAssign (this Expression left, Expression right) {
			return Expression.OrAssign (left, right);
		}

		public static BinaryExpression OrAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.OrAssign (left, right, method);
		}

		public static BinaryExpression OrAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.OrAssign (left, right, method, conversion);
		}

		public static BinaryExpression ExclusiveOr (this Expression left, Expression right) {
			return Expression.ExclusiveOr (left, right);
		}

		public static BinaryExpression ExclusiveOr (this Expression left, Expression right, MethodInfo method) {
			return Expression.ExclusiveOr (left, right, method);
		}

		public static BinaryExpression ExclusiveOrAssign (this Expression left, Expression right) {
			return Expression.ExclusiveOrAssign (left, right);
		}

		public static BinaryExpression ExclusiveOrAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.ExclusiveOrAssign (left, right, method);
		}

		public static BinaryExpression ExclusiveOrAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.ExclusiveOrAssign (left, right, method, conversion);
		}

		public static BinaryExpression Power (this Expression left, Expression right) {
			return Expression.Power (left, right);
		}

		public static BinaryExpression Power (this Expression left, Expression right, MethodInfo method) {
			return Expression.Power (left, right, method);
		}

		public static BinaryExpression PowerAssign (this Expression left, Expression right) {
			return Expression.PowerAssign (left, right);
		}

		public static BinaryExpression PowerAssign (this Expression left, Expression right, MethodInfo method) {
			return Expression.PowerAssign (left, right, method);
		}

		public static BinaryExpression PowerAssign (this Expression left, Expression right, MethodInfo method, LambdaExpression conversion) {
			return Expression.PowerAssign (left, right, method, conversion);
		}

		public static BinaryExpression ArrayIndex (this Expression array, Expression index) {
			return Expression.ArrayIndex (array, index);
		}

		public static BlockExpression Block (this Expression arg0, Expression arg1) {
			return Expression.Block (arg0, arg1);
		}

		public static BlockExpression Block (this Expression arg0, Expression arg1, Expression arg2) {
			return Expression.Block (arg0, arg1, arg2);
		}

		public static BlockExpression Block (this Expression arg0, Expression arg1, Expression arg2, Expression arg3) {
			return Expression.Block (arg0, arg1, arg2, arg3);
		}

		public static BlockExpression Block (this Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4) {
			return Expression.Block (arg0, arg1, arg2, arg3, arg4);
		}

		public static CatchBlock Catch (this ParameterExpression variable, Expression body) {
			return Expression.Catch (variable, body);
		}

		public static CatchBlock Catch (this ParameterExpression variable, Expression body, Expression filter) {
			return Expression.Catch (variable, body, filter);
		}

		public static ConditionalExpression Condition (this Expression test, Expression ifTrue, Expression ifFalse) {
			return Expression.Condition (test, ifTrue, ifFalse);
		}

		public static ConditionalExpression Condition (this Expression test, Expression ifTrue, Expression ifFalse, Type type) {
			return Expression.Condition (test, ifTrue, ifFalse, type);
		}

		public static ConditionalExpression IfThen (this Expression test, Expression ifTrue) {
			return Expression.IfThen (test, ifTrue);
		}

		public static ConditionalExpression IfThenElse (this Expression test, Expression ifTrue, Expression ifFalse) {
			return Expression.IfThenElse (test, ifTrue, ifFalse);
		}

		public static IndexExpression MakeIndex (this Expression instance, PropertyInfo indexer, IEnumerable<Expression> arguments) {
			return Expression.MakeIndex (instance, indexer, arguments);
		}

		public static IndexExpression ArrayAccess (this Expression array, params Expression[] indexes) {
			return Expression.ArrayAccess (array, indexes);
		}

		public static IndexExpression ArrayAccess (this Expression array, IEnumerable<Expression> indexes) {
			return Expression.ArrayAccess (array, indexes);
		}

		public static IndexExpression Property (this Expression instance, String propertyName, params Expression[] arguments) {
			return Expression.Property (instance, propertyName, arguments);
		}

		public static IndexExpression Property (this Expression instance, PropertyInfo indexer, params Expression[] arguments) {
			return Expression.Property (instance, indexer, arguments);
		}

		public static IndexExpression Property (this Expression instance, PropertyInfo indexer, IEnumerable<Expression> arguments) {
			return Expression.Property (instance, indexer, arguments);
		}

		public static InvocationExpression Invoke (this Expression expression, params Expression[] arguments) {
			return Expression.Invoke (expression, arguments);
		}

		public static InvocationExpression Invoke (this Expression expression, IEnumerable<Expression> arguments) {
			return Expression.Invoke (expression, arguments);
		}

		public static Expression<TDelegate> Lambda<TDelegate> (this Expression body, params ParameterExpression[] parameters) {
			return Expression.Lambda<TDelegate> (body, parameters);
		}

		public static Expression<TDelegate> Lambda<TDelegate> (this Expression body, Boolean tailCall, params ParameterExpression[] parameters) {
			return Expression.Lambda<TDelegate> (body, tailCall, parameters);
		}

		public static Expression<TDelegate> Lambda<TDelegate> (this Expression body, IEnumerable<ParameterExpression> parameters) {
			return Expression.Lambda<TDelegate> (body, parameters);
		}

		public static Expression<TDelegate> Lambda<TDelegate> (this Expression body, Boolean tailCall, IEnumerable<ParameterExpression> parameters) {
			return Expression.Lambda<TDelegate> (body, tailCall, parameters);
		}

		public static Expression<TDelegate> Lambda<TDelegate> (this Expression body, String name, IEnumerable<ParameterExpression> parameters) {
			return Expression.Lambda<TDelegate> (body, name, parameters);
		}

		public static Expression<TDelegate> Lambda<TDelegate> (this Expression body, String name, Boolean tailCall, IEnumerable<ParameterExpression> parameters) {
			return Expression.Lambda<TDelegate> (body, name, tailCall, parameters);
		}

		public static LambdaExpression Lambda (this Expression body, params ParameterExpression[] parameters) {
			return Expression.Lambda (body, parameters);
		}

		public static LambdaExpression Lambda (this Expression body, Boolean tailCall, params ParameterExpression[] parameters) {
			return Expression.Lambda (body, tailCall, parameters);
		}

		public static LambdaExpression Lambda (this Expression body, IEnumerable<ParameterExpression> parameters) {
			return Expression.Lambda (body, parameters);
		}

		public static LambdaExpression Lambda (this Expression body, Boolean tailCall, IEnumerable<ParameterExpression> parameters) {
			return Expression.Lambda (body, tailCall, parameters);
		}

		public static LambdaExpression Lambda (this Expression body, String name, IEnumerable<ParameterExpression> parameters) {
			return Expression.Lambda (body, name, parameters);
		}

		public static LambdaExpression Lambda (this Expression body, String name, Boolean tailCall, IEnumerable<ParameterExpression> parameters) {
			return Expression.Lambda (body, name, tailCall, parameters);
		}

		public static ListInitExpression ListInit (this NewExpression newExpression, params Expression[] initializers) {
			return Expression.ListInit (newExpression, initializers);
		}

		public static ListInitExpression ListInit (this NewExpression newExpression, IEnumerable<Expression> initializers) {
			return Expression.ListInit (newExpression, initializers);
		}

		public static ListInitExpression ListInit (this NewExpression newExpression, MethodInfo addMethod, params Expression[] initializers) {
			return Expression.ListInit (newExpression, addMethod, initializers);
		}

		public static ListInitExpression ListInit (this NewExpression newExpression, MethodInfo addMethod, IEnumerable<Expression> initializers) {
			return Expression.ListInit (newExpression, addMethod, initializers);
		}

		public static ListInitExpression ListInit (this NewExpression newExpression, params ElementInit[] initializers) {
			return Expression.ListInit (newExpression, initializers);
		}

		public static ListInitExpression ListInit (this NewExpression newExpression, IEnumerable<ElementInit> initializers) {
			return Expression.ListInit (newExpression, initializers);
		}

		public static LoopExpression Loop (this Expression body) {
			return Expression.Loop (body);
		}

		public static LoopExpression Loop (this Expression body, LabelTarget @break) {
			return Expression.Loop (body, @break);
		}

		public static LoopExpression Loop (this Expression body, LabelTarget @break, LabelTarget @continue) {
			return Expression.Loop (body, @break, @continue);
		}

		public static MemberExpression Field (this Expression expression, FieldInfo field) {
			return Expression.Field (expression, field);
		}

		public static MemberExpression Field (this Expression expression, String fieldName) {
			return Expression.Field (expression, fieldName);
		}

		public static MemberExpression Field (this Expression expression, Type type, String fieldName) {
			return Expression.Field (expression, type, fieldName);
		}

		public static MemberExpression Property (this Expression expression, String propertyName) {
			return Expression.Property (expression, propertyName);
		}

		public static MemberExpression Property (this Expression expression, Type type, String propertyName) {
			return Expression.Property (expression, type, propertyName);
		}

		public static MemberExpression Property (this Expression expression, PropertyInfo property) {
			return Expression.Property (expression, property);
		}

		public static MemberExpression Property (this Expression expression, MethodInfo propertyAccessor) {
			return Expression.Property (expression, propertyAccessor);
		}

		public static MemberExpression PropertyOrField (this Expression expression, String propertyOrFieldName) {
			return Expression.PropertyOrField (expression, propertyOrFieldName);
		}

		public static MemberExpression MakeMemberAccess (this Expression expression, MemberInfo member) {
			return Expression.MakeMemberAccess (expression, member);
		}

		public static MemberInitExpression MemberInit (this NewExpression newExpression, params MemberBinding[] bindings) {
			return Expression.MemberInit (newExpression, bindings);
		}

		public static MemberInitExpression MemberInit (this NewExpression newExpression, IEnumerable<MemberBinding> bindings) {
			return Expression.MemberInit (newExpression, bindings);
		}

		public static MethodCallExpression Call (this Expression instance, MethodInfo method) {
			return Expression.Call (instance, method);
		}

		public static MethodCallExpression Call (this Expression instance, MethodInfo method, params Expression[] arguments) {
			return Expression.Call (instance, method, arguments);
		}

		public static MethodCallExpression Call (this Expression instance, MethodInfo method, Expression arg0, Expression arg1) {
			return Expression.Call (instance, method, arg0, arg1);
		}

		public static MethodCallExpression Call (this Expression instance, MethodInfo method, Expression arg0, Expression arg1, Expression arg2) {
			return Expression.Call (instance, method, arg0, arg1, arg2);
		}

		public static MethodCallExpression Call (this Expression instance, String methodName, Type[] typeArguments, params Expression[] arguments) {
			return Expression.Call (instance, methodName, typeArguments, arguments);
		}

		public static MethodCallExpression Call (this Expression instance, MethodInfo method, IEnumerable<Expression> arguments) {
			return Expression.Call (instance, method, arguments);
		}

		public static MethodCallExpression ArrayIndex (this Expression array, params Expression[] indexes) {
			return Expression.ArrayIndex (array, indexes);
		}

		public static MethodCallExpression ArrayIndex (this Expression array, IEnumerable<Expression> indexes) {
			return Expression.ArrayIndex (array, indexes);
		}

		public static SwitchCase SwitchCase (this Expression body, params Expression[] testValues) {
			return Expression.SwitchCase (body, testValues);
		}

		public static SwitchCase SwitchCase (this Expression body, IEnumerable<Expression> testValues) {
			return Expression.SwitchCase (body, testValues);
		}

		public static SwitchExpression Switch (this Expression switchValue, params SwitchCase[] cases) {
			return Expression.Switch (switchValue, cases);
		}

		public static SwitchExpression Switch (this Expression switchValue, Expression defaultBody, params SwitchCase[] cases) {
			return Expression.Switch (switchValue, defaultBody, cases);
		}

		public static SwitchExpression Switch (this Expression switchValue, Expression defaultBody, MethodInfo comparison, params SwitchCase[] cases) {
			return Expression.Switch (switchValue, defaultBody, comparison, cases);
		}

		public static SwitchExpression Switch (this Expression switchValue, Expression defaultBody, MethodInfo comparison, IEnumerable<SwitchCase> cases) {
			return Expression.Switch (switchValue, defaultBody, comparison, cases);
		}

		public static TryExpression TryFault (this Expression body, Expression fault) {
			return Expression.TryFault (body, fault);
		}

		public static TryExpression TryFinally (this Expression body, Expression @finally) {
			return Expression.TryFinally (body, @finally);
		}

		public static TryExpression TryCatch (this Expression body, params CatchBlock[] handlers) {
			return Expression.TryCatch (body, handlers);
		}

		public static TryExpression TryCatchFinally (this Expression body, Expression @finally, params CatchBlock[] handlers) {
			return Expression.TryCatchFinally (body, @finally, handlers);
		}

		public static TypeBinaryExpression TypeIs (this Expression expression, Type type) {
			return Expression.TypeIs (expression, type);
		}

		public static TypeBinaryExpression TypeEqual (this Expression expression, Type type) {
			return Expression.TypeEqual (expression, type);
		}

		public static UnaryExpression Negate (this Expression expression) {
			return Expression.Negate (expression);
		}

		public static UnaryExpression Negate (this Expression expression, MethodInfo method) {
			return Expression.Negate (expression, method);
		}

		public static UnaryExpression UnaryPlus (this Expression expression) {
			return Expression.UnaryPlus (expression);
		}

		public static UnaryExpression UnaryPlus (this Expression expression, MethodInfo method) {
			return Expression.UnaryPlus (expression, method);
		}

		public static UnaryExpression NegateChecked (this Expression expression) {
			return Expression.NegateChecked (expression);
		}

		public static UnaryExpression NegateChecked (this Expression expression, MethodInfo method) {
			return Expression.NegateChecked (expression, method);
		}

		public static UnaryExpression Not (this Expression expression) {
			return Expression.Not (expression);
		}

		public static UnaryExpression Not (this Expression expression, MethodInfo method) {
			return Expression.Not (expression, method);
		}

		public static UnaryExpression IsFalse (this Expression expression) {
			return Expression.IsFalse (expression);
		}

		public static UnaryExpression IsFalse (this Expression expression, MethodInfo method) {
			return Expression.IsFalse (expression, method);
		}

		public static UnaryExpression IsTrue (this Expression expression) {
			return Expression.IsTrue (expression);
		}

		public static UnaryExpression IsTrue (this Expression expression, MethodInfo method) {
			return Expression.IsTrue (expression, method);
		}

		public static UnaryExpression OnesComplement (this Expression expression) {
			return Expression.OnesComplement (expression);
		}

		public static UnaryExpression OnesComplement (this Expression expression, MethodInfo method) {
			return Expression.OnesComplement (expression, method);
		}

		public static UnaryExpression TypeAs (this Expression expression, Type type) {
			return Expression.TypeAs (expression, type);
		}

		public static UnaryExpression Unbox (this Expression expression, Type type) {
			return Expression.Unbox (expression, type);
		}

		public static UnaryExpression Convert (this Expression expression, Type type) {
			return Expression.Convert (expression, type);
		}

		public static UnaryExpression Convert (this Expression expression, Type type, MethodInfo method) {
			return Expression.Convert (expression, type, method);
		}

		public static UnaryExpression ConvertChecked (this Expression expression, Type type) {
			return Expression.ConvertChecked (expression, type);
		}

		public static UnaryExpression ConvertChecked (this Expression expression, Type type, MethodInfo method) {
			return Expression.ConvertChecked (expression, type, method);
		}

		public static UnaryExpression ArrayLength (this Expression array) {
			return Expression.ArrayLength (array);
		}

		public static UnaryExpression Quote (this Expression expression) {
			return Expression.Quote (expression);
		}

		public static UnaryExpression Throw (this Expression value) {
			return Expression.Throw (value);
		}

		public static UnaryExpression Throw (this Expression value, Type type) {
			return Expression.Throw (value, type);
		}

		public static UnaryExpression Increment (this Expression expression) {
			return Expression.Increment (expression);
		}

		public static UnaryExpression Increment (this Expression expression, MethodInfo method) {
			return Expression.Increment (expression, method);
		}

		public static UnaryExpression Decrement (this Expression expression) {
			return Expression.Decrement (expression);
		}

		public static UnaryExpression Decrement (this Expression expression, MethodInfo method) {
			return Expression.Decrement (expression, method);
		}

		public static UnaryExpression PreIncrementAssign (this Expression expression) {
			return Expression.PreIncrementAssign (expression);
		}

		public static UnaryExpression PreIncrementAssign (this Expression expression, MethodInfo method) {
			return Expression.PreIncrementAssign (expression, method);
		}

		public static UnaryExpression PreDecrementAssign (this Expression expression) {
			return Expression.PreDecrementAssign (expression);
		}

		public static UnaryExpression PreDecrementAssign (this Expression expression, MethodInfo method) {
			return Expression.PreDecrementAssign (expression, method);
		}

		public static UnaryExpression PostIncrementAssign (this Expression expression) {
			return Expression.PostIncrementAssign (expression);
		}

		public static UnaryExpression PostIncrementAssign (this Expression expression, MethodInfo method) {
			return Expression.PostIncrementAssign (expression, method);
		}

		public static UnaryExpression PostDecrementAssign (this Expression expression) {
			return Expression.PostDecrementAssign (expression);
		}

		public static UnaryExpression PostDecrementAssign (this Expression expression, MethodInfo method) {
			return Expression.PostDecrementAssign (expression, method);
		}

	}
}
