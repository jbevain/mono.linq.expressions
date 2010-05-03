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