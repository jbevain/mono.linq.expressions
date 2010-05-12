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

		[Test]
		public void PopExpression ()
		{
			var a = Expression.Parameter (typeof (int), "a");
			var b = Expression.Parameter (typeof (int), "b");

			var lambda = Expression.Lambda<Action<int, int>> (Expression.Add (a, b), a, b);

			AssertExpression (@"
void (int a, int b)
{
	a + b;
}
", lambda);
		}

		[Test]
		public void ReturnFromIfElse ()
		{
			var a = Expression.Parameter (typeof (int), "a");
			var b = Expression.Parameter (typeof (int), "b");

			var c = Expression.Parameter (typeof (int), "c");
			var d = Expression.Parameter (typeof (int), "d");

			var lambda = Expression.Lambda<Func<int, int, int>> (
				Expression.Condition (
				Expression.GreaterThan (a, b),
				Expression.Block (
					new [] { c },
					Expression.Assign (c, Expression.Multiply (a, b)),
					c),
				Expression.Block (
					new [] { d },
					Expression.Assign (d, Expression.Divide (a, b)),
				d)),
				a, b);

			AssertExpression (@"
int (int a, int b)
{
	if (a > b)
	{
		int c;

		c = a * b;
		return c;
	}
	else
	{
		int d;

		d = a / b;
		return d;
	}
}
", lambda);
		}

		[Test]
		public void ComplexBooleanLogicalExpression ()
		{
			var a = Expression.Parameter (typeof (bool), "a");
			var b = Expression.Parameter (typeof (bool), "b");
			var c = Expression.Parameter (typeof (bool), "c");

			var lambda = Expression.Lambda<Func<bool, bool, bool, bool>> (Expression.AndAlso (a, Expression.OrElse (b, c)), a, b, c);

			AssertExpression (@"
bool (bool a, bool b, bool c)
{
	return a && (b || c);
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