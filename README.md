Undefault.NET
============

**Undefault.NET** is a small utility library that provides an interface for the configuration of certain .NET framework default implementations.  

Injection points are provided for assigning a custom implementation to `EqualityComparer<T>.Default` and `Comparer<T>.Default`.  These properties play a major role in determining the default behavior of: .NET collections, tuple classes, anonymous objects and LINQ extension methods.  


Inspiration
-----------
I wrote the code for the initial commit in response to [a stackoverflow question](http://stackoverflow.com/questions/7633260/typedelegator-equality-inconsistency/13559804#13559804) with concerns about inconsistencies in how .NET's default `IEqualityComparer<Type>` handles equality.  Specifically, the default implementation is asymmetrical, which violates the [definition of equality](http://en.wikipedia.org/wiki/Equality_(mathematics)).  My stackoverflow answer provides some explanation of this code and its usage, but I have made a few changes to the API prior to publishing on GitHub.   

Example
-------
Below is an example usage of this library that demonstrates how to configure the equality comparer for `System.Double` to round each value prior to comparing the values.  One non-obvious feature that **I'd like to point out** is that assigning a custom default `EqualityComparer<T>` for a given type automatically applies a modification to the default `Comparer<T>` to ensure compatibility.  The rule is: for any two `T` values, `x` and `y`, if `EqualityComparer<T>.Default.Equals(x,y) == true` then `Comparer<T>.Default.Compare(x,y) == 0`.

*WARNING: Please understand that this example is contrived solely for the purpose of demonstration, and that altering the way that primitives work is almost certainly a terrible idea.  This library applies modifications at the level of the entire `AppDomain` and could potentially affect the functionality of any and all referenced assemblies.*
  
        void Main()
	{	
		var array = new []{ 0.8d, 1.55d, 2.2d };
		
		Console.WriteLine(array.Contains(2d));  // False
		Console.WriteLine(Array.IndexOf(array,2d));  // -1 
		Console.WriteLine(Comparer<double>.Default.Compare(array[1],array[2]));  // -1
		
		Undefault
			.ComparisonConfigurator
			.ConfigureEqualityComparer<double>(new RoundingEqualityComparer());
		
		Console.WriteLine(array.Contains(2d));  // True
		Console.WriteLine(Array.IndexOf(array,2d));  // 1 
		Console.WriteLine(Comparer<double>.Default.Compare(array[1],array[2]));  // 0
	}
	
	public class RoundingEqualityComparer : EqualityComparer<double>
	{
		public RoundingEqualityComparer(){} 
		public override bool Equals(double x, double y){ return Math.Round(x).Equals(Math.Round(y)); }
		public override int GetHashCode(double x){ return Math.Round(x).GetHashCode(); }
	}

The library also provides a method for reverting configurations back to the .NET defaults:

		Undefault.ComparisonConfigurator.RevertConfigurationFor<double>();
