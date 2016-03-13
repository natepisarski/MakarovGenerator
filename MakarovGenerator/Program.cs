using System;
using HumDrum.Collections.Markov;
using HumDrum.Collections;

namespace MakarovGenerator
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Woah buddy. You're running something that won't be ready for a really long time.");
			Console.WriteLine ("But for other goodies, check out https://www.github.com/natepisarski");

			Console.WriteLine ("But here's some testing data...");
			var m = new Markov<string> (Transformations.Make<string> ("a", "b", "b", "C", "d", "a", "e", "b", "d"));
		}
	}
}
