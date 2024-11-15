using Eb3yrLib;
using Eb3yrLib.Extensions;
using Eb3yrLib.Maths;
using Eb3yrLib.Aerodynamics;
using static Eb3yrLib.Aerodynamics.PotentialFlows;
using Playground;
using System.Collections;
using MathNet.Numerics;
using System.Numerics;
using ScottPlot;

double[] x = Enumerable.Range(0, 12).Select(x => (double)x).ToArray();
double[] fx = x.Select(x => (double)(x * x - 2 * x - 3)).ToArray();

StreamFunction psi = (PotentialFlows.Vortex(6 * double.Pi, -1, 0) + PotentialFlows.Source(-8 * double.Pi, 2, 0) + PotentialFlows.Uniform(5, 0)).Sum();
psi = PotentialFlows.Vortex(10, 0, 0);
static RootedCoordinateVector[] CoordVecHelper((double x, double z)[,] coords, (double u, double w)[,] vecs, double maskAbove = 20)
{
	var coordVecs = new RootedCoordinateVector[coords.Length];
	int i = 0;
	var cEnum = coords.GetEnumerator();
	var vEnum = vecs.GetEnumerator();

	while (cEnum.MoveNext() && vEnum.MoveNext())
	{
		var (x, z) = ((double x, double z))cEnum.Current;
		var (u, w) = ((double u, double w))vEnum.Current;
		if (double.Abs(double.Min(u, w)) > maskAbove)
			u = w = 0;
		
		coordVecs[i] = new(new Coordinates(x, z), new Vector2((float)u, (float)w));
		i++;
	}
	return coordVecs;
}

static Coordinates3d[,] Coord3DHelper((double x, double z)[,] coords, double[,] stream, double maskAbove = 100)
{
	var arr = new Coordinates3d[coords.GetLength(0), coords.GetLength(1)];
	for (int i = 0; i < arr.GetLength(0); i++)
		for (int j = 0; j < arr.GetLength(1); j++)
			arr[i, j] = new Coordinates3d(coords[i, j].x, coords[i, j].z, stream[i, j]);

	return arr;
}

var plt = new Plot();
var grid = PotentialFlows.CoordGrid(-4, 4, -2, 2, 0.02, 0.02);

double gamma = -2.4 * double.Pi;
psi = Uniform(4.7) + Source(1.6);
psi = psi.Sum();

Console.WriteLine(psi(2, 4.1) - psi(1, 2.5));

var stream = PotentialFlows.StreamGrid(grid, psi);
var c3d = Coord3DHelper(grid, stream);	// This and stream show that I'm getting the correct stream values for x and z -> uniform depends solely on z
//plt.Add.Heatmap(c3d);	// No good, stinky, arranges based on array index, NOT on coordinate? Even though it still has coordinates? what
var cl = plt.Add.ContourLines(c3d, 50); // : )
cl.LabelStyle.IsVisible = false;
cl.Colormap = new ScottPlot.Colormaps.Turbo();
cl.LineWidth = 2;
cl.LineStyle.Rounded = true;
plt.SavePng("pltTest.png", 1600, 1200);


static unsafe bool Test(int** arr, int r, int c)	// Only takes jagged arrays
{
	_ = arr[r][c];
	return true;
}

Memory<int> mem = new([1, 2, 3, 4, 5, 6, 7, 8]);

var VelFunc = psi.GetVelocityFunc();
Console.WriteLine(VelFunc(2.3, 0));

Console.WriteLine("\nDone");
Console.ReadLine();

namespace Playground
{

}