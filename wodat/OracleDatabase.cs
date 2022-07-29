using System;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace wodat
{
    public class OracleDatabase
    {
        public string[] ERROR_RETURN_LIST = { "12505", "12537", "End of file", "01017","12533" };
        public string[] TARGET_UNAVAILABLE = { "target host or object does not exist", "handler with matching protocol stack" };
        public string[] SYSDBA_CREDS = { "28009", "SYSDBA or SYSOPER" };
        public Arguments cArgs;
        public OracleDatabase(Arguments oArgs)
        {
            this.cArgs = oArgs;
            this.GenerateConnectionString();
        }

        /*
        Generate Oracle Database connection string
        If username is not given, it is taken from args
        If password is not given, it is taken from args
        Return Connection string according to args and parameters(user, password)
        */
        public void GenerateConnectionString(string username, string password)
        {
            string ouser = username;
            string opass = password;
            string oconString = "";

            if (username is null)
            {
                ouser = cArgs.Username;
            }
            if (password is null)
            {
                opass = cArgs.Password;
            }

            //  TODO: add additional checks for empty servicename, empty SIDS


            if (cArgs.ServiceName != null)
            {
                Console.WriteLine("TNS Connection string mode enabled and SERVICE NAME used for connection string");
                cArgs.ConString = String.Format("user id={0};password={1};data source=" +
                "(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
                "(HOST={2})(PORT={3}))(CONNECT_DATA=" +
                "(ServiceName={4})))", ouser, opass, cArgs.ServerIP, cArgs.Port, cArgs.ServiceName);
            }
            else
            {
                Console.WriteLine("TNS Connection string mode enabled and SID used for connection string");
                cArgs.ConString = String.Format("user id={0};password={1};data source=" +
                 "(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
                "(HOST={2})(PORT={3}))(CONNECT_DATA=" +
                "(SID={4})))", ouser, opass, cArgs.ServerIP, cArgs.Port, cArgs.SID);
            }

            Console.WriteLine(String.Format("Oracle connection string: {0}", cArgs.ConString));

        }
        /*
            Generate Oracle Database connection string
            If username is not given, it is taken from args
            If password is not given, it is taken from args
               DOES NOT PRINT ANYTHING
            Return Connection string according to args and parameters(user, password)
        */
        public void GenerateConnectionString()
        {
            string ouser = cArgs.Username;
            string opass = cArgs.Password; 
            string oconString = "";

            if (cArgs.ServiceName != null)
            {
                cArgs.ConString = String.Format("user id={0};password={1};data source=" +
                "(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
                "(HOST={2})(PORT={3}))(CONNECT_DATA=" +
                "(ServiceName={4})))", ouser, opass, cArgs.ServerIP, cArgs.Port, cArgs.ServiceName);
            }
            else
            {
                
                cArgs.ConString = String.Format("user id={0};password={1};data source=" +
                 "(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
                "(HOST={2})(PORT={3}))(CONNECT_DATA=" +
                "(SID={4})))", ouser, opass, cArgs.ServerIP, cArgs.Port, cArgs.SID);
            }

  

        }

        public void reconConnString()
        {
            cArgs.ConString = String.Format("data source=" +
            "(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
            "(HOST={0})(PORT={1}))(CONNECT_DATA=" +
            "(COMMAND=VERSION)))", cArgs.ServerIP, cArgs.Port);
        }

        /*
         Connection to the database
        'The threaded argument is expected to be a boolean expression which indicates whether or not Oracle
        should use the mode OCI_THREADED to wrap accesses to connections with a mutex. Doing so in single threaded
        applications imposes a performance penalty of about 10-15% which is why the default is False.'
        If stopIfError == True, stop if connection error
        */

        // TODO: add some control around encoding
        // TODO: add handling SYSDBA, SYSOPER connections
        // TODO: add additional error handling
        

        public object connectDB()
        {
            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = cArgs.ConString;
               
                try
                {
                  
                    connection.Open();
                    cArgs.Dbcon = connection;         
                    Console.WriteLine("[!] -- DB Connection Success!");
                    connection.Close();
                    return "true";
                }
                catch (OracleException ex)
                {
                    if (SYSDBA_CREDS.Any(ex.Message.ToLowerInvariant().Contains))
                    {
                        return "28009";
                    }
                    else if (ERROR_RETURN_LIST.Any(ex.Message.ToLowerInvariant().Contains))
                        {
                        return ex.Message.ToString();
                    }
                    else if (TARGET_UNAVAILABLE.Any(ex.Message.ToLowerInvariant().Contains))
                    {
                        return "TARGET_UNAVAILABLE";
                    }
                    else
                    {
                        Console.WriteLine(ex.ToString());
                        return "false";
                       // Console.ReadLine();
                       // throw;
                    }

                   
                }

            }
        }

        /* 
           Returns True when the TNS listener is well configured and it can be used for connection. Otherwise, return False
           Sends a connection with an invalid login, password and SID. If TNS listener is working, the TNS listener
           should returns an error with the SID. Ib this case, the TNS listener is working. Otherwise, TNS does not work well.
        */
        

        public bool isWorkingTNSList()
        {
            bool workingTNS = false;
            var lastServiceName = cArgs.ServiceName;
            cArgs.ServiceName = null;
            var lastSID = cArgs.SID;
            cArgs.SID = "ERTUICSLAPIE";
            Console.WriteLine(String.Format("[!] -- Checking if {0}:{1} is a working TNS listener...", cArgs.ServerIP, cArgs.Port));
            GenerateConnectionString("ERTUICS", "PASSWD");
            var status = connectDB();

            if (status.ToString().Contains("ORA-12505"))
                {
                    workingTNS = true;
                }
            else
            {
                Console.WriteLine(status.ToString());
            }

            cArgs.SID = lastSID;
            cArgs.ServiceName = lastServiceName;

            return workingTNS;

        }

        public bool reconWorkingTNSList()
        {
            bool workingTNS = false;
            var lastServiceName = cArgs.ServiceName;
            cArgs.ServiceName = null;
            var lastSID = cArgs.SID;
            cArgs.SID = "ERTUICSLAPIE";
            //Console.WriteLine(String.Format("[!] -- Checking if {0}:{1} is a working TNS listener...", cArgs.ServerIP, cArgs.Port));
            cArgs.Username = "ERTUICS";
            cArgs.Password = "PASSWD";
            GenerateConnectionString();
            var status = connectDB();

            if (status.ToString().Contains("ORA-12505"))
            {
                workingTNS = true;
            }
            else
            {
                Console.WriteLine(status.ToString());
            }

            cArgs.SID = lastSID;
            cArgs.ServiceName = lastServiceName;

            return workingTNS;

        }

        // TODO: add other methods to OracleDatabase class
    }
}

