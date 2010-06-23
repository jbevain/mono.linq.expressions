using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using NUnit.Framework;

namespace Mono.Linq.Expressions {

	public abstract class BaseExpressionTest {

		[MethodImpl (MethodImplOptions.NoInlining)]
		protected static void AssertLambda<TDelegate> (string expected, Expression body, params ParameterExpression [] parameters) where TDelegate : class
		{
			var name = GetTestCaseName ();

			AssertExpression (expected, Expression.Lambda<TDelegate> (body, name, parameters));
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static string GetTestCaseName ()
		{
			var stack_trace = new StackTrace ();
			var stack_frame = stack_trace.GetFrame (2);

			return stack_frame.GetMethod ().Name;
		}

		protected static void AssertExpression (string expected, LambdaExpression expression)
		{
			var result = new StringWriter ();
			var csharp = new CSharpWriter (new TextFormatter (result));

			csharp.Write (expression);

			Assert.AreEqual (Normalize (expected), Normalize (result.ToString ()));
		}

		static string Normalize (string @string)
		{
			return @string.Replace ("\r\n", "\n").Trim ();
		}
	}
}
