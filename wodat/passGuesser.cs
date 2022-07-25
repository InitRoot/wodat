using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wodat
{
    public class passGuesser
    {
		public Arguments cArgs;
		public String fileName;
		public string[] comboList;
		public List<string> validsList = new List<string>();
		public passGuesser(Arguments nArgs, String fileName)
		{
			this.cArgs = nArgs;
			this.fileName = fileName;
		}

		//Returns valid list of creds.
		public List<string> validCreds()
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

		public bool testCredential()
        {
			var response = "";
			bool success = false;
		
			OracleDatabase nDB = new OracleDatabase(cArgs);
			response = (String)nDB.connectDB();
			if (response.Contains("TARGET_UNAVAILABLE"))
			{
				Console.WriteLine("\n [x] -- TARGET_UNAVAILABLE You might want to cancel CTRL + C..");
				return success;
			}
			else if (response.Contains("rue"))
			{			
					success = true;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("\t Testing: " + cArgs.Username + ":" + cArgs.Password + " \t State: " + response);
					validsList.Add(cArgs.Username + ":" + cArgs.Password);
					Console.ResetColor();
					return success;
			}
			else { Console.WriteLine("\t Testing: " + cArgs.Username + ":" + cArgs.Password + " \t State: " + response); return success; }

		}

		public void runPasswordGuesser()
        {
			// Let's test the file and read what the user provided
			Console.WriteLine("[?] -- Please select which type of file has been provided: \n A - Username:Password \n B - Usernames \n C - Passwords \n D - Username as Pass");
			Console.Write("> ");
			String optType = Console.ReadLine().ToUpper();
			if (optType != null)
            {
				if (optType == "A")
				{

					loadFromFile();
					Console.WriteLine("[!] -- Now attempting to connect using [" +  comboList.Count() + "] unique credential combos...");
					foreach (string combo in comboList)
                    {
						String user = combo.Split(':')[0];
						String pass = combo.Split(':')[1];

						cArgs.Username = user;
						cArgs.Password = pass;

						testCredential();
					}

					if (validsList.Count > 0)
					{
						Console.WriteLine("[!] -- Found [" + validsList.Count() + "] set of credentials!" );
						Console.WriteLine(validsList);
					}
				}
				else if ((optType == "B") && (cArgs.Password != null))
				{
					loadFromFile();
					Console.WriteLine("[!] -- Now attempting to connect using [" + comboList.Count() + "] unique usernames with the password: [" + cArgs.Password + "]" );
					foreach (string combo in comboList)
					{
						String user = combo;
						cArgs.Username = user;

						testCredential();
					}

					if (validsList.Count > 0)
					{
						Console.WriteLine("[!] -- Found [" + validsList.Count() + "] set of credentials!");
						Console.WriteLine(validsList);
					}


				}
				else if ((optType == "C")  && (cArgs.Username != null))
				{
					loadFromFile();
					Console.WriteLine("[!] -- Now attempting to connect using [" + comboList.Count() + "] unique passwords with the username: [" + cArgs.Username + "]");
					foreach (string combo in comboList)
					{
						String pass = combo;
						cArgs.Password = pass;

						testCredential();
					}

					if (validsList.Count > 0)
					{
						Console.WriteLine("[!] -- Found [" + validsList.Count() + "] set of credentials!");
						Console.WriteLine(validsList);
					}


				}
				else if (optType == "D") 
				{
					loadFromFile();
					Console.WriteLine("[!] -- Now attempting to connect using [" + comboList.Count() + "] unique usernames as passwords");
					foreach (string combo in comboList)
					{
						String user = combo;
						cArgs.Username = user;
						cArgs.Password = user;

						testCredential();
					}

					if (validsList.Count > 0)
					{
						Console.WriteLine("[!] -- Found [" + validsList.Count() + "] set of credentials!");
						validsList.ForEach(Console.WriteLine);
					}


				}
				else
                {
					Console.WriteLine("[x] -- Option not recognized! \n B -- Ensure password argument is provided. \n C -- Ensure username argument is provided. \n Exiting...");
				}

			}
            else
            {
				Console.WriteLine("[x] -- No option provided! Exiting...");
			}



		}

	}
}
