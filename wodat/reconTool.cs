
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
        public Arguments cArgs;
		public String targRecon;
		public string[] comboList;
		int tested = 0;
		public List<string> validsList = new List<string>();


		public reconTool(Arguments nArgs, String targRecon)
		{
			this.cArgs = nArgs;
			this.targRecon = targRecon;
		}

		//Returns valid list of SIDS.
		public List<string> validTargets()
		{
			return validsList;
		}

		/*
		Load data from file.
		Impossible to have duplicate data.
		*/
		public void loadFromFile()
		{
			comboList = (System.IO.File.ReadAllLines(targRecon)).Distinct().ToArray();
		}

        /*
        	Returns True if it is a working TNS listener. Otherwise False
	        Use server and port of args only for testing.
            TODO: Cleanup the exception handling
        */
        public void checkListener(Arguments cArgs)
        {
			tested = tested + 1;
			//Console.WriteLine(cArgs.ServerIP);	
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
				loadFromFile();
				Console.WriteLine("[!] -- Now attempting to discover valid TSN listeners against [" + comboList.Count() + "] targets loaded from file.");
				Parallel.ForEach(comboList, combo =>
				{
					//wrap in try catch for in case something is off with the target provided
					try
					{

						if (combo.Contains(","))
						{
							cArgs.ServerIP = combo.Split(',')[0];
							cArgs.Port = Convert.ToInt32(combo.Split(',')[1]);
							checkListener(cArgs);

						}
						else
						{
							cArgs.ServerIP = combo.Split(',')[0];
							cArgs.Port = 1521; //default port
							checkListener(cArgs);
						}
					}
					catch
					{
						//no need for errors just continue
					}

				});


			}
            else
            {

				try
				{
						IPRange range;
						range = new IPRange(targRecon);
					Console.WriteLine("[!] -- Now attempting to discover valid TSN listeners against [" + range.GetAllIP().Count() + "] targets.");
					Parallel.ForEach(range.GetAllIP(), ipa => {
						try
						{
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
