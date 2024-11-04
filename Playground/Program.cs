using Eb3yrLib;
using Eb3yrLib.Extensions;
using Eb3yrLib.Maths;
using Playground;

int[] x = Enumerable.Range(0, 12).ToArray();
double[] fx = x.Select(x => (double)(x * x - 2 * x - 3)).ToArray();



Console.WriteLine("\nDone");
Console.ReadLine();

namespace Playground
{

}