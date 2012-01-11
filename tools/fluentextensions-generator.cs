using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

class FluentExtensionsGenerator {

	static void Main (string [] args) {

		Console.WriteLine (header);
		Console.WriteLine ();
		Console.WriteLine ("// generated");
		Console.WriteLine ();
		Console.WriteLine ("using System;");
		Console.WriteLine ("using System.Collections.Generic;");
		Console.WriteLine ("using System.Runtime.CompilerServices;");
		Console.WriteLine ("using System.Linq.Expressions;");
		Console.WriteLine ("using System.Reflection;");
		Console.WriteLine ();
		Console.WriteLine ("namespace Mono.Linq.Expressions {");
		Console.WriteLine ();
		Console.WriteLine ("\tpublic static class FluentExtensions {");
		Console.WriteLine ();
		foreach (var method in typeof (Expression).GetMethods (BindingFlags.Public | BindingFlags.Static).Where (ShouldConvertMethod)) {
			Console.WriteLine ("\t\t{0} {{", MethodDeclaration (method));
			Console.WriteLine ("\t\t\treturn {0};", MethodCall (method));
			Console.WriteLine ("\t\t}");
			Console.WriteLine ();
		}

		Console.WriteLine("\t}");

		Console.WriteLine("}");
	}

	static bool ShouldConvertMethod (MethodInfo method)
	{
		var parameters = method.GetParameters ();
		if (parameters.Length == 0)
			return false;

		if (IsParams (parameters [0]))
			return false;

		if (method.Name.StartsWith ("Try"))
			return false;

		return true;
	}

	static string MethodCall (MethodInfo method)
	{
		return string.Format ("Expression.{0}{1} ({2})", method.Name, GenericParameters (method), Arguments (method));
	}

	static string Arguments (MethodInfo method)
	{
		return string.Join (", ", method.GetParameters ().Select (ParameterName).ToArray ());
	}

	static string MethodDeclaration (MethodInfo method)
	{
		return string.Format ("public static {0} {1}{2} (this {3})", TypeName (method.ReturnType), method.Name, GenericParameters (method), Parameters (method));
	}

	static string GenericParameters (MethodInfo method)
	{
		return method.IsGenericMethodDefinition ? string.Format ("<{0}>", method.GetGenericArguments () [0].Name) : "";
	}

	static string Parameters (MethodInfo method)
	{
		return string.Join (", ", method.GetParameters ().Select (p => string.Format ("{0}{1} {2}", Params (p), TypeName (p.ParameterType), ParameterName (p))).ToArray ());
	}

	static string Params (ParameterInfo parameter)
	{
		return IsParams (parameter) ? "params " : "";
	}

	static bool IsParams (ParameterInfo parameter)
	{
		return Attribute.IsDefined (parameter, typeof (ParamArrayAttribute));
	}

	static string ParameterName (ParameterInfo parameter)
	{
		switch (parameter.Name) {
		case "break":
		case "continue":
		case "finally":
			return "@" + parameter.Name;
		}

		return parameter.Name;
	}

	static string TypeName (Type type)
	{
		switch (type.Name) {
		case "Expression`1":
			return "Expression<" + type.GetGenericArguments () [0].Name + ">";
		case "IEnumerable`1":
			return "IEnumerable<" + type.GetGenericArguments () [0].Name + ">";
		case "Boolean":
			return "bool";
		case "Object":
			return "object";
		case "Int32":
			return "int";
		case "String":
			return "string";
		}

		return type.Name;
	}

	const string header =
@"//
// FluentExtensions.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2011 Novell, Inc. (http://www.novell.com)
// (C) 2012 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// ""Software""), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//";
}
