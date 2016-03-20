using System;
using System.IO;
using System.Collections.Generic;

using HumDrum.Collections;
using HumDrum.Collections.Markov;

using HumDrum.Recursion;

using HumDrum.Operations.Files;

namespace MakarovGenerator
{
	/// <summary>
	/// Takes in the text from a directory's files, 
	/// turning them into states which a markov chain can use.
	/// </summary>
	public class MarkovServer
	{
		/// <summary>
		/// The chain generated from the text files
		/// </summary>
		/// <value>The chain</value>
		private Markov<string> Chain {get; set;}

		/// <summary>
		/// The lock for this server
		/// </summary>
		private Object CurrentLock = new Object();

		/// <summary>
		/// The directory that this server controls
		/// </summary>
		/// <value>The directory</value>
		public string Directory { get; set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="MakarovGenerator.MarkovServer"/> class.
		/// Will attempt to read all files in a given directory to build a Markov chain
		/// </summary>
		/// <param name="directory">Directory.</param>
		public MarkovServer (string directory)
		{
			Directory = directory;
			Reload ();
		}

		/// <summary>
		/// Reload this instance.
		/// </summary>
		public void Reload()
		{
			string dataset = "";

			foreach (Line s in new DirectorySearch(Directory, System.IO.SearchOption.TopDirectoryOnly).LinesWhere(x => true))
				dataset += (s.Text + " ");

			Chain = new Markov<string> (dataset.Split (' '), 1);
			Console.WriteLine ("Markov Chain successfully generated from source directory");
		}

		/// <summary>
		/// Adds a file to this server
		/// </summary>
		/// <param name="filename">The filename to add</param>
		public void AddFile(string filename)
		{
			string dataset = "";

			foreach (string s in File.ReadAllLines(filename))
				dataset += (s + " ");

			Chain.AppendChain (dataset.Split (' '), 1);
		}

		/// <summary>
		/// Gets a sequence of states from the Markov Chain
		/// that begin with this state
		/// </summary>
		/// <returns>The chain of subsequent states</returns>
		/// <param name="initialState">The initial state</param>
		/// <param name="elements">How many elements this chain should generate</param>
		public IEnumerable<string> GetChain(IEnumerable<string> initialState, int elements)
		{
			lock (CurrentLock) {
				return Chain.SelectRandomSequence (initialState, elements);
			}
		}

		/// <summary>
		/// Gets a sequence of states from a random state
		/// </summary>
		/// <returns>The chain of elements</returns>
		/// <param name="elements">Elements.</param>
		public IEnumerable<string> GetChain(int elements)
		{
			return GetChain (Chain.RandomState (), elements);
		}
	}
}

