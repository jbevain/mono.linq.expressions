using System;
using System.IO;
using System.Linq.Expressions;

using NUnit.Framework;

namespace Mono.Linq.Expressions {

	[TestFixture]
	public class CSharpWriterTest {

		[Test]
		public void Add ()
		{
			var x = Expression.Parameter (typeof (int), "x");
			var y = Expression.Parameter (typeof (int), "y");

			var lambda = Expression.Lambda<Func<int, int, int>> (Expression.Add (x, y), x, y);

			AssertExpression (@"
int (int x, int y)
{
	return x + y;
}
", lambda);
		}

		[Test]
		public void AssignVariable ()
		{
			var a = Expression.Parameter (typeof (int), "a");
			var b = Expression.Parameter (typeof (int), "b");

			var c = Expression.Parameter (typeof (int), "c");

			var lambda = Expression.Lambda<Func<int, int, int>> (
				Expression.Block (
					new [] {c},
					Expression.Assign (c, Expression.AddChecked (a, b)),
					c),
				a, b);

			AssertExpression (@"
int (int a, int b)
{
	int c;

	c = checked { a + b };
	return c;
}
", lambda);
		}

		static void AssertExpression (string expected, Expression expression)
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