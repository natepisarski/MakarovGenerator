using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

using HumDrum.Collections.Markov;
using HumDrum.Collections.StateModifiers;
using HumDrum.Collections;

using HumDrum.Recursion;

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
		public const int CLIENT_PORT = 4207;

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
			Console.WriteLine ("MakarovGenerator client X |state| text1 text2 text3...");
			Console.WriteLine ();

			Console.WriteLine ("To ask MakarovGenerator to analyze a file, do this: ");
			Console.WriteLine ("MakarovGenerator evaluate file X state");
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// Gives the user the option to start a distributor or 
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			// To start off, there are three possible arguments.
			// These are "evaluate", "distributor", and "client"

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
				chain = serve.GetChain (TailHelper.Wrap(args [3]), int.Parse (args [2]));

			Console.WriteLine (Sections.RepairString (chain));
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
			Servitor.Send (Sections.RepairString (args.Tail()), "127.0.0.1", SERVER_PORT);
			TcpListener tcl = new TcpListener (IPAddress.Loopback, CLIENT_PORT);
			tcl.Start ();
			// Wait for the response
			var server_response = tcl.AcceptTcpClient();
			StreamReader reader = new StreamReader (server_response.GetStream ());

			for (string line = ""; (line = reader.ReadLine ()) != null;)
				Console.WriteLine (line);
			
			server_response.Close ();

		}
	}
}
