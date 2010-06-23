using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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

		[Test]
		public void BinaryAssignOperators ()
		{
			var a = Expression.Parameter (typeof (int), "a");
			var b = Expression.Parameter (typeof (int), "b");

			var lambda = Expression.Lambda<Func<int, int, int>> (
				Expression.Block (
					Expression.AddAssign (a, b),
					Expression.AndAssign (a, b),
					Expression.DivideAssign (a, b),
					Expression.ExclusiveOrAssign (a, b),
					Expression.LeftShiftAssign (a, b),
					Expression.MultiplyAssign (a, b),
					Expression.OrAssign (a, b),
					Expression.RightShiftAssign (a, b),
					Expression.SubtractAssign (a, b),
					a),
				a, b);

			AssertExpression (@"
int (int a, int b)
{
	a += b;
	a &= b;
	a /= b;
	a ^= b;
	a <<= b;
	a *= b;
	a |= b;
	a >>= b;
	a -= b;
	return a;
}
", lambda);
		}

		[Test]
		public void Power ()
		{
			var d = Expression.Parameter (typeof (double), "d");
			var e = Expression.Parameter (typeof (double), "e");

			var lambda = Expression.Lambda<Func<double, double, double>> (
				Expression.Power (d, e),
				d, e);

			AssertExpression (@"
double (double d, double e)
{
	return Math.Pow(d, e);
}
", lambda);
		}

		[Test]
		public void PowerAssign ()
		{
			var d = Expression.Parameter (typeof (double), "d");
			var e = Expression.Parameter (typeof (double), "e");

			var lambda = Expression.Lambda<Func<double, double, double>> (
				Expression.Block (
					Expression.PowerAssign (d, e),
					d),
				d, e);

			AssertExpression (@"
double (double d, double e)
{
	d = Math.Pow(d, e);
	return d;
}
", lambda);
		}

		[Test]
		public void Invocation ()
		{
			var i = Expression.Parameter (typeof (int), "i");
			var f = Expression.Parameter (typeof (Func<int, int>), "f");

			var lambda = Expression.Lambda<Func<int, Func<int, int>, int>> (
				Expression.Invoke (f, i),
				i, f);

			AssertExpression (@"
int (int i, Func<int, int> f)
{
	return f(i);
}
", lambda);
		}

		[Test]
		public void Coalesce ()
		{
			var a = Expression.Parameter (typeof (string), "a");
			var b = Expression.Parameter (typeof (string), "b");

			var lambda = Expression.Lambda<Func<string, string, string>> (
				Expression.Coalesce (a, b),
				a, b);

			AssertExpression (@"
string (string a, string b)
{
	return a ?? b;
}
", lambda);
		}

		[Test]
		public void ThrowNewNotImplementedException ()
		{
			var lambda = Expression.Lambda<Action> (
				Expression.Throw (
					Expression.New (typeof (NotImplementedException))));

			AssertExpression (@"
void ()
{
	throw new NotImplementedException();
}
", lambda);
		}

		[Test]
		public void IsTrueOrIsFalse ()
		{
			var b = Expression.Parameter (typeof (bool), "b");

			var lambda = Expression.Lambda<Func<bool, bool>> (
				Expression.OrElse (
					Expression.IsTrue (b),
					Expression.IsFalse (b)),
				b);

			AssertExpression (@"
bool (bool b)
{
	return b == true || b == false;
}
", lambda);
		}

		[Test]
		public void ArrayLength ()
		{
			var array = Expression.Parameter (typeof (int []), "array");

			var lambda = Expression.Lambda<Func<int[], int>> (
				Expression.ArrayLength (array),
				array);

			AssertExpression (@"
int (int[] array)
{
	return array.Length;
}
", lambda);
		}

		[Test]
		public void ArrayIndex ()
		{
			var array = Expression.Parameter (typeof (int []), "array");

			var lambda = Expression.Lambda<Func<int [], int>> (
				Expression.ArrayIndex (array, Expression.Constant (0)),
				array);

			AssertExpression (@"
int (int[] array)
{
	return array[0];
}
", lambda);
		}

		[Test]
		public void NewArrayInit ()
		{
			var lambda = Expression.Lambda<Func<string []>> (
				Expression.NewArrayInit (
					typeof (string),
					Expression.Constant ("foo"),
					Expression.Constant ("bar"),
					Expression.Constant ("baz")));

			AssertExpression (@"
string[] ()
{
	return new string[] {""foo"", ""bar"", ""baz""};
}
", lambda);
		}

		[Test]
		public void NewArrayBounds ()
		{
			var lambda = Expression.Lambda<Func<string [,,]>> (
				Expression.NewArrayBounds (
					typeof (string),
					Expression.Constant (2),
					Expression.Constant (2),
					Expression.Constant (2)));

			AssertExpression (@"
string[,,] ()
{
	return new string[2, 2, 2];
}
", lambda);
		}

		[Test]
		public void Indexer ()
		{
			var list = Expression.Parameter (typeof (List<string>), "list");
			var index = Expression.Parameter (typeof (int), "i");

			var lambda = Expression.Lambda<Func<List<string>, int, string>> (
				Expression.Property (list, "Item", index),
				list, index);

			AssertExpression (@"
string (List<string> list, int i)
{
	return list[i];
}
", lambda);
		}

		[Test]
		public void TernaryConditional ()
		{
			var b = Expression.Parameter (typeof (bool), "b");

			var expression = Expression.Condition (b, Expression.Constant ('t'), Expression.Constant ('f'));

			AssertLambda<Func<bool, char>> (@"
char TernaryConditional(bool b)
{
	return b ? 't' : 'f';
}
", expression, b);
		}

		[Test]
		public void ListInitList ()
		{
			var list_string_add = typeof (List<string>).GetMethod ("Add");

			var expression = Expression.ListInit (
				Expression.New (typeof (List<string>)),
				Expression.ElementInit (list_string_add, Expression.Constant ("foo")),
				Expression.ElementInit (list_string_add, Expression.Constant ("bar")));

			AssertLambda<Func<List<string>>> (@"
List<string> ListInitList()
{
	return new List<string>() {""foo"", ""bar""};
}
", expression);
		}

		[Test]
		public void ListInitDict ()
		{
			var dict_string_int_add = typeof (Dictionary<string, int>).GetMethod ("Add");

			var expression = Expression.ListInit (
				Expression.New (typeof (Dictionary<string, int>)),
				Expression.ElementInit (dict_string_int_add, Expression.Constant ("foo"), Expression.Constant (1)),
				Expression.ElementInit (dict_string_int_add, Expression.Constant ("bar"), Expression.Constant (2)));

			AssertLambda<Func<Dictionary<string, int>>> (@"
Dictionary<string, int> ListInitDict()
{
	return new Dictionary<string, int>() {{""foo"", 1}, {""bar"", 2}};
}
", expression);
		}

		public class Foo {
			public int Bar { get; set; }
			public List<int> Baz { get; set; }
			public Gazonk Gaz { get; set; }
		}

		public class Gazonk {
			public int Identifier { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void MemberInit ()
		{
			var list_int_add = typeof (List<int>).GetMethod ("Add");

			var expression =  Expression.MemberInit (
				Expression.New (typeof (Foo)),
				Expression.Bind (typeof (Foo).GetProperty ("Bar"), Expression.Constant (42)),
				Expression.ListBind (typeof (Foo).GetProperty ("Baz"),
					Expression.ElementInit (list_int_add, Expression.Constant (4)),
					Expression.ElementInit (list_int_add, Expression.Constant (12))),
				Expression.MemberBind (typeof (Foo).GetProperty ("Gaz"),
					Expression.Bind (typeof (Gazonk).GetProperty ("Identifier"), Expression.Constant (42)),
					Expression.Bind (typeof (Gazonk).GetProperty ("Name"), Expression.Constant ("Gazonka"))));

			AssertLambda<Func<Foo>> (@"
Foo MemberInit()
{
	return new Foo()
	{
		Bar = 42,
		Baz = {4, 12},
		Gaz =
		{
			Identifier = 42,
			Name = ""Gazonka""
		}
	};
}
", expression);
		}

		[Test]
		public void TypeAs ()
		{
			var s = Expression.Parameter (typeof (string), "s");

			var expression = Expression.TypeAs (s, typeof (object));

			AssertLambda<Func<string, object>> (@"
object TypeAs(string s)
{
	return s as object;
}
", expression, s);
		}

		[Test]
		public void TypeEqual ()
		{
			var o = Expression.Parameter (typeof (object), "o");
			var expression = Expression.TypeEqual (o, typeof (List<int>));

			AssertLambda<Func<object, bool>> (@"
bool TypeEqual(object o)
{
	return o.GetType() == typeof(List<int>);
}
", expression, o);
		}

		[Test]
		public void TypeIs ()
		{
			var o = Expression.Parameter (typeof (object), "o");
			var expression = Expression.TypeIs (o, typeof (List<int>));

			AssertLambda<Func<object, bool>> (@"
bool TypeIs(object o)
{
	return o is List<int>;
}
", expression, o);
		}

		static Expression CallWrite (object operand)
		{
			return Expression.Call (typeof (Console).GetMethod ("WriteLine", new [] { operand.GetType () }), Expression.Constant (operand));
		}

		[Test]
		public void ExceptionHandling ()
		{
			var i = Expression.Parameter (typeof (int), "i");
			var nre = Expression.Parameter (typeof (NullReferenceException), "e");

			var body = Expression.Block (
				CallWrite (0),
				Expression.MakeTry (null,
					CallWrite (1),
					CallWrite (4),
					null,
					new [] {
						Expression.Catch (typeof (OutOfMemoryException), CallWrite (2), Expression.GreaterThan (i, Expression.Constant (10))),
						Expression.Catch (nre, CallWrite (3))
					}));

			AssertLambda<Action<int>> (@"
void ExceptionHandling(int i)
{
	Console.WriteLine(0);
	try
	{
		Console.WriteLine(1);
	}
	catch (OutOfMemoryException) if (i > 10)
	{
		Console.WriteLine(2);
	}
	catch (NullReferenceException e)
	{
		Console.WriteLine(3);
	}
	finally
	{
		Console.WriteLine(4);
	}
}
", body, i);
		}

		[Test]
		public void Loop ()
		{
			var @break = Expression.Label ();
			var @continue = Expression.Label ();

			var i = Expression.Parameter (typeof (int), "i");
			var body = Expression.Block (
				new [] { i },
				Expression.Assign (i, Expression.Constant (0)),
				Expression.Loop (
					Expression.Block (
						Expression.Condition (
							Expression.NotEqual (
								Expression.Modulo (i, Expression.Constant (2)),
								Expression.Constant (0)),
							Expression.Continue (@continue),
							Expression.Call (typeof (Console).GetMethod ("WriteLine", new [] { typeof (int) }), i)),
						Expression.Assign (i, Expression.Add (i, Expression.Constant (1))),
						Expression.Condition (
							Expression.LessThan (i, Expression.Constant (10)),
							Expression.Continue (@continue),
							Expression.Break (@break))),
					@break,
					@continue));

			AssertLambda<Action> (@"
void Loop()
{
	int i;

	i = 0;
	for (;;)
	{
		if ((i % 2) != 0)
		{
			continue;
		}
		else
		{
			Console.WriteLine(i);
		}
		i = i + 1;
		if (i < 10)
		{
			continue;
		}
		else
		{
			break;
		}
	}
}
", body);
		}

		[Test]
		public void Switch ()
		{
			var i = Expression.Parameter (typeof (int), "i");

			var body = Expression.Switch (
				i,
				i,
				Expression.SwitchCase (
					Expression.Add (i, Expression.Constant (2)),
					Expression.Constant (0)),
				Expression.SwitchCase (
					Expression.Multiply (i, Expression.Constant (4)),
					Expression.Constant (2),
					Expression.Constant (4)));

			AssertLambda<Func<int, int>> (@"
int Switch(int i)
{
	switch (i)
	{
		case 0:
		{
			return i + 2;
		}
		case 2:
		case 4:
		{
			return i * 4;
		}
		default:
		{
			return i;
		}
	}
}
", body, i);
		}

		[MethodImpl (MethodImplOptions.NoInlining)]
		static void AssertLambda<TDelegate> (string expected, Expression body, params ParameterExpression [] parameters) where TDelegate : class
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
