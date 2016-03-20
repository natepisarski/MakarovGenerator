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
			MarkovDistributor dist = new MarkovDistributor ("/home/nate/TEST_DIRECTORY");
			dist.Manage ("here is some text is it too much for you to understand or is it okay?");
			var x = dist.GetChain (Math.Abs("here is some text is it too much for you to understand or is it okay?".GetHashCode ()).ToString(), TailHelper.Wrap ("is"), 10);
			foreach (string s in x) {
				Console.WriteLine (s);
			}
		}
	}
}
