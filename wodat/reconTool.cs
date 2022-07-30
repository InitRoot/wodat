
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IpRanges;

namespace wodat
{
    public class reconTool
    {

		public String targRecon;
		public IEnumerable<string> comboList;
		int tested = 0;
		public List<string> validsList = new List<string>();


		public reconTool(String targRecon)
		{
			this.targRecon = targRecon;
		}

		//Returns valid list of targets.
		public List<string> validTargets()
		{
			return validsList.Distinct().ToList();
		}


		/*
        	Returns True if it is a working TNS listener. Otherwise False
	        Use server and port of args only for testing.
            TODO: Cleanup the exception handling
        */
		public void checkListener(Arguments cArgs)
        {
			tested = tested + 1;
			//Console.WriteLine("Testing manually: " + cArgs.ServerIP);	
			if (cArgs.Port == 0)
            {
				cArgs.Port = 1521;

			}
            var statusWorking = false;
            Socket socket;
            IPAddress test1 = IPAddress.Parse(cArgs.ServerIP);
            IPEndPoint ipe = new IPEndPoint(test1, cArgs.Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.SendTimeout = 2;
			var socketConn = false;
            try
            {
		
				socket.Connect(ipe);


			}
            catch (Exception ex)
            {
				//Console.WriteLine("[x] -- ERROR Unexpected exception : {0}", e.ToString());
				// throw;
				
			}          
			if (socket.Connected)
			{
				OracleDatabase oDB = new OracleDatabase(cArgs);
				statusWorking = oDB.reconWorkingTNSList();

				if (statusWorking == false)
				{
					socket.Close();
				
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Green;
					string targ1 = cArgs.ServerIP + ":" + Convert.ToString(cArgs.Port);
					Console.WriteLine("\t Found valid target: " + targ1);
					validsList.Add(targ1);
					Console.ResetColor();
					socket.Close();
					Console.WriteLine("[!] -- Targets tested: " + tested.ToString());

				}



			}
            else { }
		
		}
            
		public void runReconTool()
        {
            if (targRecon.Contains("\\") && File.Exists(targRecon))
            {
				IEnumerable<string> comboList = File.ReadAllLines(targRecon);
				//Console.WriteLine(comboList);
				Console.WriteLine("[!] -- Now attempting to discover valid TNS listeners against [" + comboList.Count() + "] targets loaded from file.");
				foreach (string combo in comboList)
				{
					combo.Replace(" ", String.Empty);
					Console.WriteLine(combo);
					//wrap in try catch for in case something is off with the target provided
					try
					{

						if (combo.Contains(","))
						{
							Arguments cArgs = new Arguments();
							cArgs.ServerIP = combo.Split(',')[0];
							cArgs.Port = Convert.ToInt32(combo.Split(',')[1]);
							checkListener(cArgs);

						}
						else
						{
							Arguments cArgs = new Arguments();
							cArgs.ServerIP = combo.Split(',')[0];
							cArgs.Port = 1521; //default port
							checkListener(cArgs);
						}
					}
					catch
					{
						//no need for errors just continue
					}

				}


			}
            else
            {

				try
				{
					IPRange range;
					range = new IPRange(targRecon);
					Console.WriteLine("[!] -- Now attempting to discover valid TNS listeners against [" + range.GetAllIP().Count() + "] targets.");

					//Parallel.ForEach(range.GetAllIP(), new ParallelOptions { MaxDegreeOfParallelism = 8 }, ipa =>
					Parallel.ForEach(range.GetAllIP(), ipa => {
					try
					{
							Arguments cArgs = new Arguments();
							cArgs.ServerIP = ipa.ToString();
							cArgs.Port = 1521; //default port
							checkListener(cArgs);
						}
						catch (Exception ex) { //Console.WriteLine(ex.ToString());
                                               }

					});


				}
				catch (Exception ex)
				{
					
					Console.WriteLine("[x] -- Error encountered, please ensure IP range is provided correctly e.g. 192.168.1.0/24! or file path is correct.");
				}


			}
					

					if (validsList.Count > 0)
					{
						Console.WriteLine("[!] -- Found [" + validsList.Count() + "] valid targets!");
						validsList.ForEach(Console.WriteLine);
			}
					else
					{
					
						Console.ReadLine();

			}
		}




    }
}
