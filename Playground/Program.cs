using Playground;
using static System.Console;

FindSetOfInts.Run();
FindSetOfInts.RunSyncTask();
await FindSetOfInts.RunAsync();

WriteLine("Done");
ReadLine();