using Eb3yrLib;
using Eb3yrLib.Maths;
using Eb3yrLib.Extensions;
using Playground;
using System.Collections;
using MathNet.Numerics.Random;
using System.Globalization;

double[] x = Enumerable.Range(0, 12).Select(x => (double)x).ToArray();
double[] fx = x.Select(x => (double)(x * x - 2 * x - 3)).ToArray();

Console.WriteLine(Maths.LerpUnsorted(11, x, fx));
Console.WriteLine(Maths.IEnumerableLerp(11, x, fx));

Console.WriteLine("\nDone");
Console.ReadLine();

namespace Playground
{

}