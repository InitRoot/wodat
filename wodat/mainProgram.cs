using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace wodat
{
    class mainProgram
    {
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

         static void Main(string[] args)
        {
            // banner
            //Console.WriteLine("#################################################");
            Console.WriteLine("WODAT - Windows Oracle Testing Toolkit");
            Console.WriteLine("@initroot");
            Console.WriteLine("#################################################");

            /* parse the arguments, split by -key
            -user
            -pass
            -sid
            -srv
            -server
            -port
            -help
            -module
            */
            if (args.Length < 1)
            {
                   Console.WriteLine("[!] -- The following arguments are required:  \n COMMAND (ALL,BRUTECRED,BRUTESID,BRUTESRV,TEST,RECON,DISC) \n -server:XXX.XXX.XXX.XXX -port:1520 \n -sid:OR -srv:OR\n -user:Peter -pass:Password");
            }

            else
            {
                var arguments = new ArgumentParser(args);
                Arguments nArgs = new Arguments();
                if (arguments.Parameters.ContainsKey("help"))
                {
                    Console.WriteLine("[!] -- The following arguments are required:  \n COMMAND (ALL,BRUTECRED,BRUTESID,BRUTESRV,TEST,RECON,DISC) \n -server:XXX.XXX.XXX.XXX -port:1520 \n -sid:OR -srv:OR\n -user:Peter -pass:Password");
                }
                // Let's make sure that server and port and minimal has been provided
                else if (arguments.Parameters.ContainsKey("server") && arguments.Parameters.ContainsKey("port"))
                {
                    //Let's check which of the commands to run
                    if (arguments.Command == "ALL")
                    {
                        nArgs.ServerIP = arguments.Parameters["server"];
                        nArgs.Port = Convert.ToInt32(arguments.Parameters["port"]);


                        if (checkListener(nArgs) == true)
                        {
                            Console.WriteLine("[!] -- ALL has not been implemented yet!");
                            //0)TNS Poinsoning


                            //A)SID MANAGEMENT


                            //A.2 SERVICE NAME MANAGEMENT


                            //B)ACCOUNT MANAGEMENT
                        }
                    }
                    else if (arguments.Command == "RECON")
                    {
                        nArgs.ServerIP = arguments.Parameters["server"];
                        nArgs.Port = Convert.ToInt32(arguments.Parameters["port"]);

                        if (checkListener(nArgs) == true)
                        {
                            Console.WriteLine("[!] -- RECON has not been implemented yet!");
                        }

                    }
                    else if (arguments.Command == "DISC")
                    {
                            // TODO: validate the data provided
                                Console.WriteLine("[?] -- Please provide file with targets or input network range: ");
                                Console.Write("> ");
                                String targRecon = Console.ReadLine();
                                targRecon = targRecon.Trim(new Char[] { '"', '*', (char)39 });
                                if (targRecon != null)
                                {
                                    reconTool rto = new reconTool(nArgs, targRecon);
                                    rto.runReconTool();
                                    Console.WriteLine("[!] -- DONE");
                                }
                                else
                                {
                                    Console.WriteLine("[x] -- File path not provided or file doesn't exist or network range not correct! Exiting...");

                                }
                    }
                    else if (arguments.Command == "BRUTECRED")
                    {
                        if (arguments.Parameters.ContainsKey("sid") || arguments.Parameters.ContainsKey("srv"))
                        {
                            // TODO: validate the data provided
                            if (arguments.Parameters.ContainsKey("sid")) { nArgs.SID = arguments.Parameters["sid"]; };
                            if (arguments.Parameters.ContainsKey("srv")) { Console.Write("SET"); nArgs.ServiceName = arguments.Parameters["srv"]; };
                            nArgs.ServerIP = arguments.Parameters["server"];
                            nArgs.Port = Convert.ToInt32(arguments.Parameters["port"]);

                            //Check if the listener is active before we proceed
                            if (checkListener(nArgs) == true)
                            {
                                Console.WriteLine("[?] -- Please provide location to file for testing: ");
                                Console.Write("> ");
                                String fileName = Console.ReadLine();
                                fileName = fileName.Trim(new Char[] { '"', '*', (char)39 });
                                if ((fileName != null) && (File.Exists(fileName)))
                                {
                                    passGuesser gs = new passGuesser(nArgs, fileName);
                                    gs.runPasswordGuesser();
                                    Console.WriteLine("[!] -- DONE");
                                }
                                else
                                {
                                    Console.WriteLine("[x] -- File path not provided or file doesn't exist! Exiting...");

                                }

                            }

                        }
                        else
                        {
                            Console.WriteLine("[x] -- Please ensure sid or servicename are given!");

                        }
                    }
                    else if (arguments.Command == "BRUTESID")
                    {

                            // TODO: validate the data provided
                            nArgs.ServiceName = null;
                            nArgs.ServerIP = arguments.Parameters["server"];
                            nArgs.Port = Convert.ToInt32(arguments.Parameters["port"]);
                            //Check if the listener is active before we proceed
                            if (checkListener(nArgs) == true)
                            {
                                Console.WriteLine("[?] -- Please provide location to file for testing: ");
                                Console.Write("> ");
                                String fileName = Console.ReadLine();
                                fileName = fileName.Trim(new Char[] { '"', '*', (char)39 });
                            if ((fileName != null) && (File.Exists(fileName)))
                                {
                                    sidGuesser sdG = new sidGuesser(nArgs, fileName);
                                    sdG.runSIDGuesser();
                                    Console.WriteLine("[!] -- DONE");
                            }
                                else
                                {
                                    Console.WriteLine("[x] -- File path not provided or file doesn't exist! Exiting...");

                                }

                            }

                       }
                    else if (arguments.Command == "BRUTESRV")
                    {

                        // TODO: validate the data provided
                        nArgs.ServiceName = null;
                        nArgs.ServerIP = arguments.Parameters["server"];
                        nArgs.Port = Convert.ToInt32(arguments.Parameters["port"]);
                        //Check if the listener is active before we proceed
                        if (checkListener(nArgs) == true)
                        {
                            Console.WriteLine("[?] -- Please provide location to file for testing: ");
                            Console.Write("> ");
                            String fileName = Console.ReadLine();
                            fileName = fileName.Trim(new Char[] { '"', '*', (char)39 });
                            if ((fileName != null) && (File.Exists(fileName)))
                            {
                                srvGuesser sdG = new srvGuesser(nArgs, fileName);
                                sdG.runSRVGuesser();
                                Console.WriteLine("[!] -- DONE");
                            }
                            else
                            {
                                Console.WriteLine("[x] -- File path not provided or file doesn't exist! Exiting...");

                            }

                        }

                    }
                    else if (arguments.Command == "TEST")
                    {
                        if (arguments.Parameters.ContainsKey("user") && arguments.Parameters.ContainsKey("pass") && (arguments.Parameters.ContainsKey("sid") || arguments.Parameters.ContainsKey("srv")))
                        {
                            // TODO: validate the data provided
                            nArgs.Username = arguments.Parameters["user"];
                            nArgs.Password = arguments.Parameters["pass"];

                            if (arguments.Parameters.ContainsKey("sid")) { nArgs.SID = arguments.Parameters["sid"]; };
                            if (arguments.Parameters.ContainsKey("srv")) { Console.Write("SET"); nArgs.ServiceName = arguments.Parameters["srv"]; };

                            nArgs.ServerIP = arguments.Parameters["server"];
                            nArgs.Port = Convert.ToInt32(arguments.Parameters["port"]);

                            //Check if the listener is active before we proceed
                            if (checkListener(nArgs) == true)
                            {
                                testConnection tDB = new testConnection(nArgs);
                                Console.WriteLine("[!] -- Attempted to connect to the instance: " + tDB.testConn().ToString());
                                Console.Write("> ");
                                Console.ReadLine();
                            }


                        }
                        else
                        {
                            Console.WriteLine("[x] -- Please ensure user, pass, [sid or servicename] are given!");

                        }
                    }
                    else
                    {
                    Console.WriteLine("[x] -- You have not entered any command!");
                    Console.WriteLine("[!] -- The following arguments are required:  \n COMMAND (ALL,BRUTECRED,BRUTESID,BRUTESRV,TEST,RECON,DISC) \n -server:XXX.XXX.XXX.XXX -port:1520 \n -sid:OR -srv:OR\n -user:Peter -pass:Password");

                    }


                }
                else
                {

                    Console.WriteLine("[!] -- The following arguments are required:  \n COMMAND (ALL,BRUTECRED,BRUTESID,BRUTESRV,TEST,RECON,DISC) \n -server:XXX.XXX.XXX.XXX -port:1520 \n -sid:OR -srv:OR\n -user:Peter -pass:Password");
                }
            }

        }
    }
}

