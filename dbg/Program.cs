using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using Mono.Linq.Expressions;

class Program {

	static void Main (string [] args)
	{
		var a = Expression.Parameter (typeof (int), "a");
		var b = Expression.Parameter (typeof (int), "b");

		var return_label = Expression.Label (typeof (int), "ret");

		var c = Expression.Parameter (typeof (int), "c");
		var d = Expression.Parameter (typeof (int), "d");

		var left = Expression.Block (
			new [] { c },
			Expression.Assign (c, Expression.AddChecked (a, b)),
			Expression.AddAssignChecked (c, Expression.Constant (42)),
			c);

		var right = Expression.Block (
			new [] { d },
			Expression.Assign (d, Expression.SubtractChecked (a, b)),
			Expression.SubtractAssignChecked (d, Expression.Constant (42)),
			d);

		var conditional = Expression.Condition (
			Expression.GreaterThan (a, b),
			left, right);

		var lambda = Expression.Lambda<Func<int, int, int>> (conditional, a, b);

		var add = lambda.Compile ();

		Console.WriteLine (add (2, 2));

		Console.WriteLine ("--------------------");

		var csharp = new CSharpWriter (new TextFormatter (Console.Out));

		csharp.Write (lambda);

		Console.WriteLine ("--------------------");

		Console.WriteLine (typeof (Expression).GetProperty ("DebugView", BindingFlags.NonPublic | BindingFlags.NonPublic | BindingFlags.Instance).GetValue (lambda, new object [0]));
	}
}

