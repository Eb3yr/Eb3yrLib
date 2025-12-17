using Playground;
using static System.Console;

FindSetOfInts.Run();
await FindSetOfInts.RunAsync();

/*
 * Results when running single core on AMD EPYC 7452. All ~560-570MB of memory.
Find			for batchSize = 2147483647	ran in	884s
Find			for batchSize = 64000		ran in	1595s
FindAsync		for batchSize = 2147483647	ran in	1098s
FindAsync		for batchSize = 64000		ran in	1991s
*/

WriteLine("Done");
ReadLine();