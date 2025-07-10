using System.Numerics;

namespace Eb3yrLib.Extensions
{
	public static class VectorExtensions
	{
		public static void Deconstruct(this Vector2 vec2, out float x, out float y)
		{
			x = vec2.X;
			y = vec2.Y;
		}

		public static void Deconstruct(this Vector3 vec3, out float x, out float y, out float z)
		{
			x = vec3.X;
			y = vec3.Y;
			z = vec3.Z;
		}

		public static void Deconstruct(this Vector4 vec4, out float x, out float y, out float z, out float w)
		{
			x = vec4.X;
			y = vec4.Y;
			z = vec4.Z;
			w = vec4.W;
		}
	}
}
