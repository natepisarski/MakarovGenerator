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
		public bool IsDirectoryControlled(string directory)
		{
			return Servers.Any (x => x.Directory.Equals (directory));
		}

		/// <summary>
		/// Tests to see if the supplied source is being controlled.
		/// </summary>
		/// <returns><c>true</c> if this instance is source controlled the specified source; otherwise, <c>false</c>.</returns>
		/// <param name="source">The source of the text</param>
		public bool IsSourceControlled(string source)
		{
			return Servers.Any(x => x.Source.Equals(source));
		}

		/// <summary>
		/// Unconditionally add the directory to the 
		/// current distributor.
		/// </summary>
		/// <param name="directory">The directory to add</param>
		public void UnconditionalAdd(string directory, string source)
		{
			Servers.Add (new MarkovServer (directory, source));
		}

		/// <summary>
		/// Returns the Source of a file, given the directory.
		/// </summary>
		/// <returns>The source of some directory. If the proper Server isn't found, this is "".</returns>
		/// <param name="source">The source for a directory</param>
		public string SourceOf(string directory)
		{
			return Servers.DoTo((x => x.Directory.Equals(directory)), (MarkovServer x) => {return x.Source;});
		}

		/// <summary>
		/// Finds the directory
		/// </summary>
		/// <returns>The of.</returns>
		/// <param name="source">Source.</param>
		public string DirectoryOf(string source)
		{
			return Servers.DoTo ((x => x.Source.Equals (source)), (MarkovServer x) => {
				return x.Directory;
			});
		}
			
		/// <summary>
		/// Returns the server watching the current directory.
		/// </summary>
		/// <returns>The server watching the current directory</returns>
		/// <param name="directory">The directory</param>
		public MarkovServer GetServerWatchingDirectory(string directory)
		{
			if (IsDirectoryControlled (directory)) {
				var serv = Servers.When (x => x.Directory.Equals (directory)).Get (0);
				serv.Reload ();
				return serv;
			} else
				UnconditionalAdd (directory, directory);
			return Servers.Get (Servers.Length () - 1);
		}

		/// <summary>
		/// Returns the server watching the given source
		/// </summary>
		/// <returns>The server watching source</returns>
		/// <param name="source">The source of the text</param>
		public MarkovServer GetServerWatchingSource(string source)
		{
			if (IsSourceControlled (source)) {
				var serv = Servers.First (x => x.Source.Equals (source));
				serv.Reload ();
				return serv;
			} else {
				UnconditionalAdd (source, source);
			}

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
			return GetServerWatchingDirectory (directory).GetChain (seed, elements);
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
		public MarkovServer Manage(string source, string text)
		{
			var potentialName = source;

			if (!(IsSourceControlled (potentialName)))
			{
				DirectorySearch ds = new DirectorySearch ("./", SearchOption.TopDirectoryOnly);

				// Keeps changing the potential name until a file like it is not in the directory.
				// i.e "name", "name0", "name01", "name012", etc
				for(int i = 0; ds.Files.Contains(potentialName); i++)
					potentialName += i;

				Directory.CreateDirectory (potentialName);
				GrowInto (potentialName, text);
				UnconditionalAdd (potentialName, source);
			}

			return GetServerWatchingDirectory (potentialName);
		}

		/// <summary>
		/// Manages a file, using the filename as the source name
		/// </summary>
		/// <returns>The markovserver</returns>
		/// <param name="filename">The name of the file</param>
		public MarkovServer ManageFile(string filename, string source)
		{
			string buffer = "";

			foreach (string s in File.ReadAllLines(filename))
				buffer += (s + " ");

			return Manage (source, buffer);		
		}

		/// <summary>
		/// Loads a directory into the Distributor
		/// </summary>
		/// <param name="directory">The directory to load</param>
		public void Load(string directory)
		{
			Servers.Add (new MarkovServer (directory, directory));
		}

		/// <summary>
		/// Loads every Directory into this MarkovDistributorn
		/// </summary>
		public void LoadAll()
		{
			DirectorySearch ds = new DirectorySearch (RootDirectory, SearchOption.TopDirectoryOnly);

			foreach (string s in ds.Files)
				Servers.Add (new MarkovServer (s, s)); // warAndPeace01 is the directory and source when loaded this way
		}
	}
}

