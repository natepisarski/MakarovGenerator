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
		public void Start()
		{
			for (;;) {
				Thread.Sleep (1000);
				if (RequestLog.Changed) {
					foreach (string r in RequestLog.Collect()) {
						Console.WriteLine ("[Wrapper]: Asked this: " + r);
						// Requests are formatted like this: "Y |state| here is a lot of text that can go on for a long time"
						// where Y is how many elements to get

						// The items of the request
						IEnumerable<string> items = Sections.ParseSections(r, '|');

						// Requests a server by hashcode
						var dirRequest = Sections.RepairString (r.Split (' ').Subsequence(2, r.Length()));

						// The server watching over the directory that dirRequest wants
						var server = Distributor.Manage (dirRequest);

						// Information about what kind of chain to get
						var desiredState = items.Get (1).Split (' ');
						var desiredLength = int.Parse (items.Get (0));

						// The chain returned from the correct server
						var values = server.GetChain (desiredState, desiredLength);

						// The repaired chain
						var returnChain = Sections.RepairString (values);

						Servitor.Send (returnChain, "127.0.0.1", ClientPort);
					}
				}
			}
		}
	}
}

