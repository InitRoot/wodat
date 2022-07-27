using System;
using System.Collections.Generic;
using System.Linq;
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
			comboList = (System.IO.File.ReadAllLines(fileName)).Distinct().ToArray();
		}

        /*
        	Returns True if it is a working TNS listener. Otherwise False
	        Use server and port of args only for testing.
            TODO: Cleanup the exception handling
        */
        public static bool checkListener(Arguments nArgs)
        {
            var statusWorking = false;
            Socket socket;
            IPAddress test1 = IPAddress.Parse(nArgs.ServerIP);
            IPEndPoint ipe = new IPEndPoint(test1, nArgs.Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ipe);
            }
            catch (ArgumentNullException ae)
            {
                Console.WriteLine("[x] -- ERROR ArgumentNullException : {0}", ae.ToString());
                throw;
            }
            catch (SocketException se)
            {
                Console.WriteLine("[x] -- ERROR SocketException : {0}", se.ToString());
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("[x] -- ERROR Unexpected exception : {0}", e.ToString());
                throw;
            }
            socket.Close();
            Console.WriteLine("[!] -- Socket connection established to target");
            OracleDatabase oDB = new OracleDatabase(nArgs);
            statusWorking = oDB.isWorkingTNSList();

            if (statusWorking == false)
            {
                Console.WriteLine("[x] -- ERROR TNS listener is NOT well configured. Exiting...");
                return false;

            }
            else
                Console.WriteLine("[!] -- SUCCESS Working TNS listener. Continue...");
                return true;

        }
		public bool reconTarget()
		{
				Thread.Sleep(2000);
				var response = "";
				bool success = false;
				OracleDatabase nDB = new OracleDatabase(cArgs);
				cArgs.Username = "POIOPI";
				cArgs.Password = "SDFEWRTER";
				nDB.GenerateConnectionString();
				response = (String)nDB.connectDB();
				if (response.Contains("TARGET_UNAVAILABLE"))
				{
					Console.WriteLine("\n [x] -- TARGET_UNAVAILABLE You might want to cancel CTRL + C..");
					return success;
				}
				else if (NO_GOOD_SID_STRING_LIST.Any(response.ToLowerInvariant().Contains))
				{
					success = true;
					return success;
				}
				else
				{
					success = true;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("\t Found potential valid SID: " + cArgs.SID + " \t State: " + response);
					validsList.Add(cArgs.SID);
					Console.ResetColor();
					return success;
				}
		}

		public void runReconTool()
        {
            if (targRecon.Contains("\\") && File.Exists(targRecon))
            {

            }
            else if ()
            {
                IPAddress ip;
                bool b = IPAddress.TryParse(targRecon,out ip);

            }
					loadFromFile();
					Console.WriteLine("[!] -- Now attempting to connect using [" + comboList.Count() + "] unique SIDs...");
					foreach (string combo in comboList)
					{
						cArgs.SID = combo.ToUpperInvariant();
						TestSID(false);
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
