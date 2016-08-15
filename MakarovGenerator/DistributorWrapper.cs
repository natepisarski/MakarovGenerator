using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

using HumDrum.Operations;
using HumDrum.Collections;

namespace MakarovGenerator
{
	/// <summary>
	/// Wraps a MarkovDistributor in a TCP-receiving wrapper.
	/// </summary>
	public class DistributorWrapper
	{
		/// <summary>
		/// The distributor that this server will use to build chains.
		/// </summary>
		/// <value>The distributor</value>
		MarkovDistributor Distributor {get; set;}

		/// <summary>
		/// A Servitor is used to buffer commands for this server to perform 
		/// so that it can perform it on a request-by-request basis
		/// </summary>
		/// <value>The request log</value>
		Servitor RequestLog { get; set; }

		/// <summary>
		/// The thread that the Servitor is running in
		/// </summary>
		/// <value>The servitor thread</value>
		Thread ServitorThread {get; set;}

		/// <summary>
		/// Gets or sets the server port.
		/// This is defined as a constant in the upper level of the program.
		/// </summary>
		/// <value>The server port.</value>
		int ServerPort { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <value>The client port.</value>
		int ClientPort {get; set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="MakarovGenerator.DistributorWrapper"/> class.
		/// This sets up the Servitor to listen to a certain port and initializes the distributor.
		/// </summary>
		public DistributorWrapper (string rootDirectory, int serverPort, int clientPort)
		{
			Distributor = new MarkovDistributor (rootDirectory);

			RequestLog = new Servitor (IPAddress.Loopback, serverPort);

			ServitorThread = new Thread (new ThreadStart (RequestLog.Start));

			ServerPort = serverPort;
			ClientPort = clientPort;

			Console.WriteLine ("[Wrapper]: Initialized properly. Servitor thread is about to initialize");
			ServitorThread.Start ();
		}

		/// <summary>
		/// Start this DistributorWrapper, feeding the
		/// Servitor's buffer into the MarkovDistributor.
		/// </summary>
		public async void Start()
		{
			for (;;) {
				Thread.Sleep (1000);
				if (RequestLog.Changed) {
					foreach (string r in await RequestLog.Collect()) {
						Console.WriteLine ("[Wrapper]: Asked this: " + r);

						/*
						 * Requests are formatted in a number of ways. Here are the possible ways for
						 * them to be formatted:
						 *		0				1				2				3
						 *    source           text          the source           word2... etc
						 *    source           file           filename
						 *    chain            source		 some number		seed1, seed2... etc         
						 * */

						// The items of the request
						IEnumerable<string> items = Sections.ParseSections(r, '|');

						// Either 'source' or 'chain'
						switch (items.Get (0)) {
						case "source":
							switch (items.Get (1)) {
							case "text":
								SourceText (items.Get (2), Transformations.Subsequence (items, 3, items.Length ()));
								break;
							case "file":
								SourceFile (items.Get (2), items.Get (2));
								break;
							}
							break;
						case "chain":
							Chain (items.Get (1), int.Parse (items.Get (2)), items.Subsequence (3, items.Length()));
							break;
						}

						Console.WriteLine ("Request processed");
					}
				}
			}
		}

		/// <summary>
		/// Analyzes some text, adding it to the source.
		/// </summary>
		/// <param name="source">The source to analyze</param>
		/// <param name="words">The words to analyze</param>
		public void SourceText(string source, IEnumerable<string> words)
		{
			Distributor.Manage (source, Sections.RepairString (words));

			Servitor.Send ("Text successfully managed", "127.0.0.1", 4206);
		}

		/// <summary>
		/// Learns by using a file
		/// </summary>
		/// <param name="source">The source name you would like to use</param>
		/// <param name="filename">The name of the file</param>
		public void SourceFile(string source, string filename)
		{
			Distributor.ManageFile (filename, source);

			Servitor.Send ("File successfully managed", "127.0.0.1", 4206);
		}

		/// <summary>
		/// Gets a Chain from the distributor
		/// </summary>
		/// <param name="source">The source directory to format from</param>
		/// <param name="seed">The seed</param>
		public void Chain(string source, int elements, IEnumerable<string> seed)
		{
			Distributor.GetChain (source, seed, elements);
		}
	}
}

