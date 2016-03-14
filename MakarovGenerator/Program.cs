using System;
using System.Collections.Generic;

using HumDrum.Collections.Markov;
using HumDrum.Collections.StateModifiers;
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
			Console.WriteLine ("Let's test out some State Objects!");
			Console.WriteLine ("Wouldn't it be neat to count to 10 with state objects?");

			IntegerCounter x = new IntegerCounter (10, 1);
			for (; (!x.ModifyState ());) {
				Console.WriteLine (x.State);
			}

			Console.WriteLine ();
			Console.WriteLine ("Now let's see if we can't get elements in pairs of 2 from a big list");

			foreach (List<int> y in Groups.Group<int> (Transformations.Make(1, 2, 3, 4, 5, 6, 7, 8, 9, 10), new IntegerCounter (6, 1))) {
				y.ForEach (w => Console.Write (w + " "));
				Console.WriteLine ();
			}

			Console.WriteLine ("Oh wow, cool! Our stateobjects just kicked some butt.");
			Console.WriteLine ("Why don't we fire up our Markov library and see if it's working as intended");
			Markov<string> markov = new Markov<string> (Transformations.Make("here", "lies", "words", "here", "lies", "sounds", "here", "are", "things"), 1);
			Console.WriteLine ("Based on state of \"lies\" of degree 1: " + markov.SelectRandom (Transformations.Make ("lies")));
		}
	}
}
