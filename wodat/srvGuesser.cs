using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wodat
{
	public class srvGuesser
	{
		public Arguments cArgs;
		public String fileName;
		public string[] comboList;
		public List<string> validsList = new List<string>();
		public string[] NO_GOOD_SRV_STRING_LIST = { "listener does not currently know of service requested", "listener does not currently know of SID", "connection to server failed", "destination host unreachable" };


		public srvGuesser(Arguments nArgs, String fileName)
		{
			this.cArgs = nArgs;
			this.fileName = fileName;
		}

		//Returns valid list of SRVS.
		public List<string> validSRVS()
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

		public bool TestSRV(bool brute)
		{
			if (brute == false)
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
				else if (NO_GOOD_SRV_STRING_LIST.Any(response.ToLowerInvariant().Contains))
				{
					success = true;
					return success;
				}
				else
				{
					success = true;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("\t Found potential valid ServiceName: " + cArgs.ServiceName + " \t State: " + response);
					validsList.Add(cArgs.ServiceName);
					Console.ResetColor();
					return success;
				}
			}
			else
			{
				Thread.Sleep(750);
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
				else if (NO_GOOD_SRV_STRING_LIST.Any(response.ToLowerInvariant().Contains))
				{
					return success;
				}
				else
				{
					success = true;
					Console.ForegroundColor = ConsoleColor.Green;
					validsList.Add(cArgs.ServiceName);
					Console.WriteLine("\t Found potential valid ServiceName: " + cArgs.ServiceName + " \t State: " + response);
					Console.ResetColor();
					return success;
				}
			}


		}

		string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public void bruteSRVs(string prefix, int level, int maxlen)
		{

			level += 1;
			foreach (char c in ValidChars)
			{
				string word = (prefix + c);
				//Console.Write("\b\b\b\b\b{0}", word);
				cArgs.ServiceName = word.ToUpperInvariant();
				TestSRV(true);
				if (level < maxlen)
				{
					bruteSRVs(prefix + c, level, maxlen);
				}
			}

		}


		// TODO: implement function when networking module works
		public void loadSRVsFromListenerAlias()
		{


		}
		public void runSRVGuesser()
		{
			loadFromFile();
			Console.WriteLine("[!] -- Now attempting to connect using [" + comboList.Count() + "] unique ServiceNames...");
			foreach (string combo in comboList)
			{
				cArgs.ServiceName = combo.ToUpperInvariant();
				TestSRV(false);
			}

			if (validsList.Count > 0)
			{
				Console.WriteLine("[!] -- Found [" + validsList.Count() + "] valid ServiceNames!");
				validsList.ForEach(Console.WriteLine);
			}
			else
			{
				Console.WriteLine("[?] -- No valid ServiceNames found from provided list... Would you like to perform bruteforce attack \t (Y - Yes | N - No)?");
				Console.Write("> ");
				String respBrute = Console.ReadLine().ToUpperInvariant();
				if (respBrute == "Y")
				{
					Console.WriteLine("[!] -- Now attempting to bruteforce 2 char ServiceNames values. Please be patient, this can take a couple of minutes... CTRL + C to quit..");
					bruteSRVs("", 0, 2);
					Console.WriteLine("[!] -- Now attempting to bruteforce 3 char ServiceNames values. Please be patient, this can take a couple of minutes... CTRL + C to quit..");
					bruteSRVs("", 0, 3);
					Console.WriteLine("[!] -- Now attempting to bruteforce 4 char ServiceNames values. Please be patient, this can take a couple of minutes... CTRL + C to quit..");
					bruteSRVs("", 0, 4);
				}
				else
				{

				}

			}
		}
	}
}


	
