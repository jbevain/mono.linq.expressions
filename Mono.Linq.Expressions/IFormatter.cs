namespace Mono.Linq.Expressions {

	public interface IFormatter {
		void Write (string str);
		void WriteLine ();
		void WriteSpace ();
		void WriteToken (string token);
		void WriteKeyword (string keyword);
		void WriteLiteral (string literal);
		void WriteReference (string value, object reference);
		void WriteIdentifier (string value, object identifier);
		void Indent ();
		void Dedent ();
	}
}
