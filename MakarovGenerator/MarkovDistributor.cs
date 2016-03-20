using System;
using System.IO;
using System.Collections.Generic;

using HumDrum.Collections;
using HumDrum.Operations.Files;
namespace MakarovGenerator
{
	/// <summary>
	/// Manages a list of MarkovServers based on directories. 
	/// This handles the creation of new diretories for certain types of data.
	/// </summary>
	public class MarkovDistributor
	{
		/// <summary>
		/// Gets or sets the root directory of this MarkovDistributor
		/// </summary>
		/// <value>The root directory as a string</value>
		public string RootDirectory { get; set;}

		/// <summary>
		/// A list of running MarkovServers
		/// </summary>
		/// <value>The currently running markov servers</value>
		public List<MarkovServer> Servers { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MakarovGenerator.MarkovDistributor"/> class.
		/// Given a root directory to run the MarkovDistributor, it manages metadata that
		/// tell the distributor whether or not a certain data source has readable information already 
		/// put into a directory or running.
		/// </summary>
		public MarkovDistributor (string root)
		{
			Servers = new List<MarkovServer> ();
			RootDirectory = root;	
		}

		/// <summary>
		/// Determines whether this instance is controlling the specified directory.
		/// </summary>
		/// <returns><c>true</c> if this instance is controlling the specified directory; otherwise, <c>false</c>.</returns>
		/// <param name="directory">The directory to test</param>
		public bool IsControlled(string directory)
		{
			foreach (MarkovServer s in Servers)
				if (s.Directory.Equals (directory))
					return true;
			return false;
		}

		/// <summary>
		/// Unconditionally add the directory to the 
		/// current distributor.
		/// </summary>
		/// <param name="directory">The directory to add</param>
		public void UnconditionalAdd(string directory)
		{
			Servers.Add (new MarkovServer (directory));
		}

		/// <summary>
		/// Add a directory to the distributor only if it's not 
		/// already added.
		/// </summary>
		/// <param name="directory">The directory to add</param>
		public void SafeAdd(string directory)
		{
			if (!IsControlled (directory))
				UnconditionalAdd (directory);
		}

		/// <summary>
		/// Returns the server watching the current directory.
		/// </summary>
		/// <returns>The server watching the current directory</returns>
		/// <param name="directory">The directory</param>
		public MarkovServer GetServerWatching(string directory)
		{
			if (IsControlled (directory)) {
				var serv = Servers.When (x => x.Directory.Equals (directory)).Get (0);
				serv.Reload ();
				return serv;
			} else
				UnconditionalAdd (directory);
			return Servers.Get (Servers.Length () - 1);
		}

		/// <summary>
		/// Gets the MarkovChain from the relevant directory
		/// </summary>
		/// <returns>The chain</returns>
		/// <param name="directory">The directory that the chain is watching</param>
		/// <param name="seed">The seed state</param>
		/// <param name="elements">The number of elements to put into the chain</param>
		public IEnumerable<string> GetChain(string directory, IEnumerable<string> seed, int elements)
		{
			return GetServerWatching (directory).GetChain (seed, elements);
		}

		/// <summary>
		/// Grow into a given directory. This will write text
		/// to a text file in this directory.
		/// </summary>
		/// <param name="directory">The directory to grow into</param>
		/// <param name="text">The text to write to a text file</param>
		public void GrowInto(string directory, string text)
		{
			new NumericalWriter().Write (directory, text, ".txt");
		}

		/// <summary>
		/// Hash the text, attempting to create a directory of that name.
		/// If the directory already exists, it returns that MarkovServer.
		/// </summary>
		/// <param name="text">The text in question</param>
		public MarkovServer Manage(string text)
		{
			var potentialName = Math.Abs(text.GetHashCode ()).ToString();

			if (!(IsControlled (potentialName)))
			{
				Directory.CreateDirectory (potentialName);
				GrowInto (potentialName, text);
				SafeAdd (potentialName);
			}

			return GetServerWatching (potentialName);
		}
	}
}

