using Eb3yrLib;
using Playground;
using System.Collections;
using System.Diagnostics;
using System.Numerics;

string str = "Jumble me please";
Console.WriteLine(str.Jumble());

string[] strs = [ "a", "b", "c", "d", "e" ];
foreach (string s in strs.Jumble())
{
	Console.Write(s);
}

string[] strs2 = [..strs.Select((string s) => s + "e")];

Func<int, int> func = x => x * x;
Console.WriteLine("\n" + func(2));
// Go benchmark a version of Jumble with singles and with doubles. 

Console.WriteLine("\nDone");
Console.ReadLine();

namespace Playground
{
	public static class StrExt
	{
		public static string Jumble(this string str)
		{
			Random rng = new();
			char[] chars = str.ToCharArray();
			return new string(chars.OrderBy((char c) => rng.NextSingle() - 0.5f).ToArray());
		}
	}

	public static class CollectionExtensions
	{
		public static ICollection<T> Jumble<T> (this ICollection<T> collection)
		{
			Random rng = new();
			return [.. collection.OrderBy((T item) => rng.NextSingle() - 0.5f)];
		}
	}
}