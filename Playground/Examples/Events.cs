using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Examples
{
	internal class Events : IExample
	{
		/// <summary>Test instance class to subscribe to an event</summary>
		private class Test(int ID = 0)
		{
			private readonly int ID = ID;

			public void PrintID(object? sender, EventArgs? e)
			{
				Console.WriteLine(ID);
			}
		}

		public void Run()
		{
			Test test = new(3);
			Test test2 = new(4);
			EventHandler? handler = null!;  // Empty event handlers are always null, making it a nullable ref type makes this clear
			handler += test.PrintID;
			handler += test2.PrintID;
			handler.Invoke(null, null!);
			handler -= test.PrintID;    // If you don't unsubscribe, the subscriber never gets garbage collected. Setting the obj ref to null doesn't affect this at all
			handler -= test.PrintID;
			handler -= test2.PrintID;
			handler?.Invoke(null, null!);   // ? operator only invokes if it isn't null
			if (handler is null)
				Console.WriteLine("null");
		}
	}
}
