## Mono.Linq.Expressions

Mono.Linq.Expressions is an helper library to complement the System.Linq.Expressions namespace.

It works on both Mono >= 2.8 and .net >= 4.0.

## API

***

```csharp
public static class CSharp {
	public string ToCSharpCode (Expression) {}
}
```

> Returns a string containing the C# representation of the expression.

***

```csharp
public static class FluentExtensions {}
```

> Provides extension methods to ease the creation of expression trees. For instance, instead of writing:

```csharp
var field = Expression.Field (
	Expression.Convert (parameter, typeof (string)),
	"Length");
```

> You can write:

```csharp
var field = parameter.Convert (typeof (string)).Field ("Length");
```

***

```csharp
public abstract class CustomExpression {
	public abstract Expression Accept (CustomExpressionVisitor visitor) {}
}

```

> Base class for custom expressions.

> Accept takes a custom visitor which extends the built-in ExpressionVisitor with support for custom expressions.

***

```csharp
public abstract class ExpressionWriter {}
```

> Provides a base class for pretty print specific language, such as CSharpWriter used by CSharp.ToCSharpCode().

***

```csharp
public class DoWhileExpression : CustomExpression {}
```

> A `do {} while (condition);` statement.

***

```csharp
public class ForEachExpression : CustomExpression {}
```

> A `foreach (var item in iterable) {}` statement.

***

```csharp
public class ForExpression : CustomExpression {}
```

> A `for (initializer; condition; increment) {}` statement.

***

```csharp
public class UsingExpression : CustomExpression {}
```

> A `using (disposable) {}` statement.

***

```csharp
public class WhileExpression : CustomExpression {}
```

> A `while (condition) {}` statement.

***

```csharp
public static class PredicateBuilder {

	public static Expression<Func<T, bool>> OrElse<T> (
		this Expression<Func<T, bool>> self,
		Expression<Func<T, bool>> expression) {}

	public static Expression<Func<T, bool>> AndAlso<T> (
		this Expression<Func<T, bool>> self,
		Expression<Func<T, bool>> expression) {}

	public static Expression<Func<T, bool>> Not<T> (
		this Expression<Func<T, bool>> self) {}
}
```

> Provides a way to combine lambda predicates using boolean operators. Expressions are rewritten to keep the predicates simple and understandable by LINQ providers. For instance:

```csharp
Expression<Func<User, bool>> isOver18 = u => u.Age > 18;
Expression<Func<User, bool>> isFemale = u => u.Gender == Gender.Female;

Expression<Func<User, bool>> femalesOver18 = isOver18.AndAlso (isFemale);

// >> femalesOver18.ToString ()
// u => u.Age > 18 && u.Gender == Gender.Female
```
