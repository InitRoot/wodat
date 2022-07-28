using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wodat
{
    public class reconTool
    {
        public Arguments cArgs;
		public String targRecon;
		public string[] comboList;
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
        public bool checkListener(Arguments cArgs)
        {
            var statusWorking = false;
            Socket socket;
            IPAddress test1 = IPAddress.Parse(cArgs.ServerIP);
            IPEndPoint ipe = new IPEndPoint(test1, cArgs.Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ipe);
            }
            catch (ArgumentNullException ae)
            {
                //Console.WriteLine("[x] -- ERROR ArgumentNullException : {0}", ae.ToString());
                throw;
            }
            catch (SocketException se)
            {
                //Console.WriteLine("[x] -- ERROR SocketException : {0}", se.ToString());
                throw;
            }
            catch (Exception e)
            {
                //Console.WriteLine("[x] -- ERROR Unexpected exception : {0}", e.ToString());
                throw;
            }
            socket.Close();
            //Console.WriteLine("[!] -- Socket connection established to target");
            OracleDatabase oDB = new OracleDatabase(cArgs);
            statusWorking = oDB.isWorkingTNSList();

            if (statusWorking == false)
            {        
                return false;

            }
            else
				Console.ForegroundColor = ConsoleColor.Green;
				string targ1 = cArgs.ServerIP + ":" + Convert.ToString(cArgs.Port);
				Console.WriteLine("\t Found valid target: " + targ1);
				validsList.Add(targ1);
				Console.ResetColor();
				return true;

        }


		public void runReconTool()
        {
            if (targRecon.Contains("\\") && File.Exists(targRecon))
            {
				loadFromFile();
				Console.WriteLine("[!] -- Now attempting to discover valid TSN listeners against [" + comboList.Count() + "] targets loaded from file.");
				foreach (string combo in comboList)
				{
					try
					{
						//wrap in try catch for in case something is off with the target provided
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

					}
				}

			}
            else if ()
            {
                IPAddress ip;
                bool b = IPAddress.TryParse(targRecon,out ip);

            }
					

					if (validsList.Count > 0)
					{
						Console.WriteLine("[!] -- Found [" + validsList.Count() + "] valid SIDs!");
						validsList.ForEach(Console.WriteLine);
			}
					else
					{
						Console.WriteLine("[?] -- No valid SIDs found from provided list... Would you like to perform bruteforce attack \t (Y - Yes | N - No)?");
						Console.Write("> ");
						String respBrute = Console.ReadLine().ToUpperInvariant();
						if (respBrute == "Y")
						{
							Console.WriteLine("[!] -- Now attempting to bruteforce 1 char SID values. Please be patient, this can take a couple of minutes... CTRL + C to quit..");
							bruteSIDs("", 0,1);
							Console.WriteLine("[!] -- Now attempting to bruteforce 2 char SID values. Please be patient, this can take a couple of minutes... CTRL + C to quit..");
							bruteSIDs("", 0,2);
							Console.WriteLine("[!] -- Now attempting to bruteforce 3 char SID values. Please be patient, this can take a couple of minutes... CTRL + C to quit..");
							bruteSIDs("", 0,3);
							Console.WriteLine("[!] -- Now attempting to bruteforce 4 char SID values. Please be patient, this can take a couple of minutes... CTRL + C to quit..");
							bruteSIDs("", 0, 4);	
				}
						else
							{

							}

			}
		}




    }
}
