//
// BaseExpressionTest.cs
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
