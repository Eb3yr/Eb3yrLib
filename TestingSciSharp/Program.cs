using MathNet.Numerics;
using MathNet.Numerics.Integration;
using NumSharp;
using OxyPlot;	// Will need to import another NuGet package for display, figure out one that works well on Linux for the laptop.

NDArray v = np.arange(0f, 1f, 0.1f);
NDArray nd = new float[5] {1f, 2f, 3f, 4f, 5f };  // Implicit cast float array to numpy array
Console.WriteLine("nd = " + nd);
float[] flt = nd.ToArray<float>();
flt = (float[])nd;
Console.WriteLine(flt.ToString());
// GORGEOUS CASTS
static double SomeFunc(double x) => 3d * x * x * x;
double d1 = Integrate.OnClosedInterval(SomeFunc, 0d, 1d);
Console.WriteLine(d1);

double[] xArr = [1, 2, 3, 4, 5];
double[] yArr = xArr.Select(SomeFunc).ToArray();



// Check https://numerics.mathdotnet.com/Integration for the different integration methods. Some can take f(x, y), some only f(x)
Console.ReadLine();