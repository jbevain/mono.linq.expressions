using System;
using System.IO;
using System.Linq.Expressions;

namespace Mono.Linq.Expressions {

	public static class CSharp {

		public static string ToCSharpCode (this Expression self)
		{
			if (self == null)
				throw new ArgumentNullException ("self");

			var @string = new StringWriter ();
			var csharp = new CSharpWriter (new TextFormatter (@string));

			csharp.Write (self);

			return @string.ToString ();
		}
	}
}
