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
		MarkovDistributor Distributor {get; set;}
		Servitor RequestLog { get; set; }
		Thread ServitorThread {get; set;}
		int ServerPort { get; set; }
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
						// Requests are formatted like this: "Y |state| here is a lot of text that can go on for a long time"
						// where Y is how many elements to get
						IEnumerable<string> items = Sections.ParseSections(r, '|');
						var server = Distributor.Manage(Sections.RepairString(r.Split(' ').Tail()));
						var values = server.GetChain (items.Get (1).Split(' '), int.Parse (items.Get (0)));
						Servitor.Send (Sections.RepairString (values), "127.0.0.1", ClientPort);
					}
				}
			}
		}
	}
}

