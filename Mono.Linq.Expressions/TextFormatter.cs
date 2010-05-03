using System;
using System.IO;

namespace Mono.Linq.Expressions {

	public class TextFormatter : IFormatter {

		readonly TextWriter writer;

		bool write_indent;
		int indent;

		public TextFormatter (TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			this.writer = writer;
		}

		void WriteIndent ()
		{
			if (!write_indent)
				return;

			for (int i = 0; i < indent; i++)
				writer.Write ("\t");
		}

		public void Write (string str)
		{
			WriteIndent ();
			writer.Write (str);
			write_indent = false;
		}

		public void WriteLine ()
		{
			writer.WriteLine ();
			write_indent = true;
		}

		public void WriteSpace ()
		{
			Write (" ");
		}

		public void WriteToken (string token)
		{
			Write (token);
		}

		public void WriteKeyword (string keyword)
		{
			Write (keyword);
		}

		public void WriteLiteral (string literal)
		{
			Write (literal);
		}

		public void WriteReference (string value, object reference)
		{
			Write (value);
		}

		public void WriteIdentifier (string value, object identifier)
		{
			Write (value);
		}

		public void Indent ()
		{
			indent++;
		}

		public void Dedent ()
		{
			indent--;
		}
	}
}
