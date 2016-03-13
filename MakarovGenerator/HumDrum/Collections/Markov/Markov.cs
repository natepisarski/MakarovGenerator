using System;
using System.Collections.Generic;

using HumDrum.Collections.Markov;

namespace HumDrum.Collections.Markov
{
	public class Markov<T>
	{
		public List<MarkovState<T>> States { get; private set; }

		public Markov (IEnumerable<T> dataset)
		{
			foreach (T item in dataset) {

			}
		}
	}
}

