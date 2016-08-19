using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

using HumDrum.Collections.Markov;
using HumDrum.Collections.StateModifiers;
using HumDrum.Collections;

using HumDrum.Operations;

namespace MakarovGenerator
{
	class MainClass
	{
		/// <summary>
		/// The default port for the server to listen to
		/// </summary>
		public const int SERVER_PORT = 4206;

		/// <summary>
		/// The default port for the client to listen to
		/// </summary>
		public const int CLIENT_PORT = 4205;

		/// <summary>
		/// If the user enters command line arguments that don't 
		/// make sense, print help for them
		/// </summary>
		public static void PrintHelp()
		{
			Console.WriteLine ("MakarovGenerator help: You can use any of the following commands\n");

			Console.WriteLine ("To start the distributor on port 4206, do this: ");
			Console.Write ("MakarovGenerator distributor DIRECTORY\n");
			Console.WriteLine();

			Console.WriteLine ("To ask the distributor to manage some text, do this: ");
			Console.WriteLine ("MakarovGenerator client source X text Y Z...., where X is where it came from, and the words follow \"text\"");
			Console.WriteLine ();

			Console.WriteLine ("To ask the distributor to manager a file, do this: ");
			Console.WriteLine ("MakarovGenerator client source X file Y");
			Console.WriteLine ();

			Console.WriteLine ("To ask the distributor to return a Markov chain, do this: ");
			Console.WriteLine ("MakarovGenerator client chain source [X], where X is some number");
			Console.WriteLine ();

			Console.WriteLine ("You can also ask MakarovGenerator to evaluate a plain file with \"evaluate\"");
			Console.WriteLine ("MakarovGenerator evaluate file [word / random] length");
			Console.WriteLine ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// Gives the user the option to start a distributor or 
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			// Everything here is either for the distributor or the client

			if (args.Length.Equals (0)) {
				PrintHelp ();
				return;
			}

			switch (args [0]) {
			case "evaluate":
				Evaluate (args);
				break;
			case "distributor":
				MakeDistributor (args);
				break;
			case "client":
				ClientSend (args);
				break;
			}

		}

		/// <summary>
		/// Evaluate based on a specific file.
		/// args format: "evaluate file length state"
		/// </summary>
		/// <param name="args">Arguments.</param>
		public static void Evaluate(string[] args)
		{
			var relevant = args.Tail ();

			MarkovServer serve = new MarkovServer ();
			serve.AddFile (relevant.Get (0));

			IEnumerable<string> chain;

			if (args [1].Equals ("random"))
				chain = serve.GetChain (int.Parse (args [2]));
			else
				chain = serve.GetChain (Transformations.Wrap(args [3]), int.Parse (args [2]));

			foreach (string word in chain)
				Console.Write (word + " ");

			Console.WriteLine ();
		}

		/// <summary>
		/// Creates a DistributorWrapper that will listen on the default port.
		/// This will then send the chain to an awaiting client (MainClass.ClientSend)
		/// </summary>
		/// <param name="args">Arguments: "distributor rootDir"</param>
		public static void MakeDistributor(string[] args)
		{
			DistributorWrapper dw = new DistributorWrapper (args [1], SERVER_PORT, CLIENT_PORT);
			dw.Start ();
		}

		/// <summary>
		/// Create a client, request something from the distributor,
		/// and print upon receiving.
		/// </summary>
		/// <param name="args">Arguments: "client X |state| text1 text2 text3...</param>
		public static void ClientSend(string[] args)
		{
			string argLine = "";

			foreach (string word in args.Tail())
				argLine += (word + " ");
			
			Servitor.Send (argLine, "127.0.0.1", SERVER_PORT);

			Console.WriteLine ("Request sent. The listening server is initializing on port 4205");
			/* Start the listening server */
			TcpListener tcpl = new TcpListener (CLIENT_PORT);
			tcpl.Start ();

			TcpClient client = tcpl.AcceptTcpClient ();

			Console.WriteLine (Servitor.LineFromClient (client));
			client.Close ();
			tcpl.Stop ();
		}
	}
}
