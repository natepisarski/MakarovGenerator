using System;
using System.Collections.Generic;

using HumDrum.Collections.Markov;
using HumDrum.Collections.StateModifiers;
using HumDrum.Collections;

using HumDrum.Recursion;

namespace MakarovGenerator
{
	class MainClass
	{
		public static void PrintHelp()
		{
			
		}

		// Must be called in the form of "makarov [directory] state"
		public static void Main (string[] args)
		{
			MarkovServer server = new MarkovServer ("/home/nate/Code/TEST_DIRECTORY");
			Console.WriteLine(Sections.RepairString(server.GetChain(50)));
			
		}
	}
}
