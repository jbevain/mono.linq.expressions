//
// CSharpWriterTest.cs
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

	[TestFixture]
	public class CSharpWriterTest : BaseExpressionTest {

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
		public void LogicalOperators ()
		{
			var a = Expression.Parameter (typeof (bool), "a");
			var b = Expression.Parameter (typeof (bool), "b");

			var lambda = Expression.Lambda<Func<bool, bool, bool>> (
				Expression.Block (
					Expression.Assign (a, Expression.AndAlso (a, b)),
					Expression.Assign (a, Expression.OrElse (a, b)),
					a),
				a, b);

			AssertExpression (@"
bool (bool a, bool b)
{
	a = a && b;
	a = a || b;
	return a;
}
", lambda);
		}

		[Test]
		public void BinaryBooleanOperators ()
		{
			var a = Expression.Parameter (typeof (int), "a");
			var b = Expression.Parameter (typeof (int), "b");
			var c = Expression.Parameter (typeof (bool), "c");

			var lambda = Expression.Lambda<Func<int, int, bool>> (
				Expression.Block (
					new [] { c },
					Expression.Assign (c, Expression.GreaterThan (a, b)),
					Expression.Assign (c, Expression.GreaterThanOrEqual (a, b)),
					Expression.Assign (c, Expression.LessThan (a, b)),
					Expression.Assign (c, Expression.LessThanOrEqual (a, b)),
					Expression.Assign (c, Expression.Equal (a, b)),
					Expression.Assign (c, Expression.NotEqual (a, b)),
					c),
				a, b);

			AssertExpression (@"
bool (int a, int b)
{
	bool c;

	c = a > b;
	c = a >= b;
	c = a < b;
	c = a <= b;
	c = a == b;
	c = a != b;
	return c;
}
", lambda);
		}


		[Test]
		public void BinaryOperators ()
		{
			var a = Expression.Parameter (typeof (int), "a");
			var b = Expression.Parameter (typeof (int), "b");

			var lambda = Expression.Lambda<Func<int, int, int>> (
				Expression.Block (
					Expression.Assign (a, Expression.Add (a, b)),
					Expression.Assign (a, Expression.And (a, b)),
					Expression.Assign (a, Expression.Divide (a, b)),
					Expression.Assign (a, Expression.ExclusiveOr (a, b)),
					Expression.Assign (a, Expression.LeftShift (a, b)),
					Expression.Assign (a, Expression.Modulo (a, b)),
					Expression.Assign (a, Expression.Multiply (a, b)),
					Expression.Assign (a, Expression.Or (a, b)),
					Expression.Assign (a, Expression.RightShift (a, b)),
					Expression.Assign (a, Expression.Subtract (a, b)),
					a),
				a, b);

			AssertExpression (@"
int (int a, int b)
{
	a = a + b;
	a = a & b;
	a = a / b;
	a = a ^ b;
	a = a << b;
	a = a % b;
	a = a * b;
	a = a | b;
	a = a >> b;
	a = a - b;
	return a;
}
", lambda);
		}

		[Test]
		public void BinaryCheckedOperators ()
		{
			var a = Expression.Parameter (typeof (int), "a");
			var b = Expression.Parameter (typeof (int), "b");

			var lambda = Expression.Lambda<Func<int, int, int>> (
				Expression.Block (
					Expression.Assign (a, Expression.AddChecked (a, b)),
					Expression.Assign (a, Expression.MultiplyChecked (a, b)),
					Expression.Assign (a, Expression.SubtractChecked (a, b)),
					a),
				a, b);

			AssertExpression (@"
int (int a, int b)
{
	a = checked { a + b };
	a = checked { a * b };
	a = checked { a - b };
	return a;
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
					Expression.ModuloAssign (a, b),
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
	a %= b;
	a *= b;
	a |= b;
	a >>= b;
	a -= b;
	return a;
}
", lambda);
		}

		[Test]
		public void BinaryAssignCheckedOperators ()
		{
			var a = Expression.Parameter (typeof (int), "a");
			var b = Expression.Parameter (typeof (int), "b");

			var lambda = Expression.Lambda<Func<int, int, int>> (
				Expression.Block (
					Expression.AddAssignChecked (a, b),
					Expression.MultiplyAssignChecked (a, b),
					Expression.SubtractAssignChecked (a, b),
					a),
				a, b);

			AssertExpression (@"
int (int a, int b)
{
	checked { a += b };
	checked { a *= b };
	checked { a -= b };
	return a;
}
", lambda);
		}

		[Test]
		public void UnaryOperators ()
		{
			var a = Expression.Parameter (typeof (int), "a");

			var lambda = Expression.Lambda<Func<int, int>> (
				Expression.Block (
					Expression.Assign (a, Expression.UnaryPlus (a)),
					Expression.Assign (a, Expression.Negate (a)),
					Expression.Assign (a, Expression.OnesComplement (a)),
					a),
				a);

			AssertExpression (@"
int (int a)
{
	a = +a;
	a = -a;
	a = ~a;
	return a;
}
", lambda);
		}

		[Test]
		public void UnaryCheckedOperators ()
		{
			var a = Expression.Parameter (typeof (int), "a");

			var lambda = Expression.Lambda<Func<int, int>> (
				Expression.Block (
					Expression.Assign (a, Expression.NegateChecked (a)),
					a),
				a);

			AssertExpression (@"
int (int a)
{
	a = checked { -a };
	return a;
}
", lambda);
		}

		[Test]
		public void UnaryLogicalOperators ()
		{
			var a = Expression.Parameter (typeof (bool), "a");

			var lambda = Expression.Lambda<Func<bool, bool>> (
				Expression.Block (
					Expression.Assign (a, Expression.Not (a)),
					a),
				a);

			AssertExpression (@"
bool (bool a)
{
	a = !a;
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
		public void ThrowNewNotSupportedException ()
		{
			var lambda = Expression.Lambda<Action> (
				Expression.Throw (
					Expression.New (typeof (NotSupportedException))));

			AssertExpression (@"
void ()
{
	throw new NotSupportedException();
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

		[Test]
		public void IncrementDecrement ()
		{
			var i = Expression.Parameter (typeof (int), "i");

			var body = Expression.Block (
				Expression.Assign (i, Expression.Increment (i)),
				Expression.Assign (i, Expression.Decrement (i)),
				Expression.PreIncrementAssign (i),
				Expression.PostIncrementAssign (i),
				Expression.PreDecrementAssign (i),
				Expression.PostDecrementAssign (i),
				i);

			AssertLambda<Func<int, int>> (@"
int IncrementDecrement(int i)
{
	i = i + 1;
	i = i - 1;
	++i;
	i++;
	--i;
	i--;
	return i;
}
", body, i);
		}

		[Test]
		public void Default ()
		{
			var body = Expression.Default (typeof (int));

			AssertLambda<Func<int>> (@"
int Default()
{
	return default(int);
}
", body);
		}

		[Test]
		public void Convert ()
		{
			var o = Expression.Parameter (typeof (object), "o");

			var body = Expression.Convert (o, typeof (string));

			AssertLambda<Func<object, string>> (@"
string Convert(object o)
{
	return (string)o;
}
", body, o);
		}

		[Test]
		public void ConvertChecked ()
		{
			var i = Expression.Parameter (typeof (int), "i");

			var body = Expression.ConvertChecked (i, typeof (short));

			AssertLambda<Func<int, short>> (@"
short ConvertChecked(int i)
{
	return checked { (short)i };
}
", body, i);
		}

		[Test]
		public void Unbox ()
		{
			var o = Expression.Parameter (typeof (object), "o");

			var body = Expression.Unbox (o, typeof (int));

			AssertLambda<Func<object, int>> (@"
int Unbox(object o)
{
	return (int)o;
}
", body, o);
		}

		[Test]
		public void QuoteLambda ()
		{
			var s = Expression.Parameter (typeof (string), "s");

			var lambda = Expression.Lambda<Func<string, Expression<Func<string>>>> (
				Expression.Quote (
					Expression.Lambda<Func<string>> (s, new ParameterExpression [0])),
				s);

			AssertExpression (@"
Expression<Func<string>> (string s)
{
	return () =>
	{
		return s;
	};
}
", lambda);
		}
	}
}
