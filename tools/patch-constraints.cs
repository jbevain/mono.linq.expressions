using System;
using System.IO;
using System.Linq;

using Mono.Cecil;

class Program {

	static void ProcessModule (ModuleDefinition module)
	{
		foreach (var type in module.Types) {
			Process (type);

			foreach (var method in type.Methods)
				Process (method);
		}
	}

	static void Process (IGenericParameterProvider provider)
	{
		if (!provider.HasGenericParameters)
			return;


		foreach (var parameter in provider.GenericParameters)
			Process (parameter);
	}

	static void Process (GenericParameter parameter)
	{
		if (!parameter.HasCustomAttributes)
			return;

		var attributes = parameter.CustomAttributes;
		for (int i = 0; i < attributes.Count; i++) {
			var attribute = attributes [i];
			if (IsDelegateConstraintAttribute (attribute)) {
				parameter.Attributes = GenericParameterAttributes.NonVariant;
				parameter.Constraints.Clear ();
				parameter.Constraints.Add (CreateConstraint ("System", "Delegate", parameter));
				attributes.RemoveAt (i--);
			} else if (IsEnumConstraintAttribute (attribute)) {
				parameter.Attributes = GenericParameterAttributes.NonVariant | GenericParameterAttributes.NotNullableValueTypeConstraint;
				parameter.Constraints.Clear ();
				parameter.Constraints.Add (CreateConstraint ("System", "Enum", parameter));
				attributes.RemoveAt (i--);
			}
		}
	}

	static TypeReference CreateConstraint (string @namespace, string name, GenericParameter parameter)
	{
		return new TypeReference (
			@namespace,
			name,
			parameter.Module,
			parameter.Module.AssemblyReferences.First<AssemblyNameReference> (a => a.Name == "mscorlib"),
			false);
	}

	static bool IsEnumConstraintAttribute (CustomAttribute attribute)
	{
		return IsConstraintAttribute ("EnumConstraintAttribute", attribute);
	}

	static bool IsDelegateConstraintAttribute(CustomAttribute attribute)
	{
		return IsConstraintAttribute ("DelegateConstraintAttribute", attribute);
	}

	static bool IsConstraintAttribute (string type, CustomAttribute attribute)
	{
		return attribute.AttributeType.Name == type;
	}

	static void Main (string [] args)
	{
		if (args.Length == 0)
			Usage ();

		var file = args [0];
		if (!File.Exists (file))
			Usage ();

		var snk = args.Length > 1 && File.Exists (args [1]) ? args [1] : null;
		var sn = snk != null ? new System.Reflection.StrongNameKeyPair (File.OpenRead (snk)) : null;

		var module = ModuleDefinition.ReadModule (file);
		ProcessModule (module);

		foreach (var attribute in new [] { "DelegateConstraintAttribute", "EnumConstraintAttribute" }) {
			var type = module.GetType (attribute);
			if (type != null)
				module.Types.Remove (type);
		}

		module.Write (file, new WriterParameters { StrongNameKeyPair = sn });
	}

	static void Usage ()
	{
		Console.WriteLine ("patch-constraints assembly [keypair.snk]");
		Environment.Exit (1);
	}
}
