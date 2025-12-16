using Playground;
using System.Collections;
using System.Diagnostics;
using static System.Console;

//var sw = Stopwatch.StartNew();
//await FindSetOfInts.FindAsync("a.bin", "b.bin", "out.bin", batchSize: int.MaxValue);
//sw.Stop();
//
//WriteLine($"Found set in {double.Round(sw.ElapsedMilliseconds / 1000d, 4)}s.");
//WriteLine($"Peak memory: {double.Round(Process.GetCurrentProcess().PeakWorkingSet64 / 1_000_000d, 3)}MB");

FindSetOfInts.Run();
FindSetOfInts.RunSyncTask();
await FindSetOfInts.RunAsync();

WriteLine("Done");
ReadLine();