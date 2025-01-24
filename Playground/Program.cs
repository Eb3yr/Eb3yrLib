using Playground;
using Eb3yrLib;
using Eb3yrLib.Extensions;
using Eb3yrLib.Mathematics;
using Eb3yrLib.Collections;
using Eb3yrLib.Aerodynamics;
using Eb3yrLib.Trees;
using System.Collections;
using System.Numerics;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;



Console.WriteLine("Done");
Console.ReadLine();

namespace Playground
{
	public class Vec2Field(Func<float, float, Vector2>? inFunction = null)
	{


		Func<float, float, Vector2> func = inFunction ?? Zero;

		public Vector2 this[float x, float y] => Invoke(x, y);

		public Vector2 Invoke(float x, float y)
		{
			var v = Vector2.Zero;
			foreach (var f in func.GetInvocationList().Cast<Func<float, float, Vector2>>())
				v += f.HasSingleTarget ? f(x, y) : v += new Vec2Field(f).Invoke(x, y);

			return v;
		}

		private static Vector2 Zero(float x, float y) => Vector2.Zero;

		public void Add(Func<float, float, Vector2> function) => func += function;

		public void Clear() => func = Zero;

		public static implicit operator Vec2Field(Func<float, float, Vector2> function) => new(function);
	}
}