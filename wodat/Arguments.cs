using System;
namespace wodat
{
	public class Arguments
	{
        private string username;
        private string password;
        private string serviceName;
        private string sID;
        private string serverIP;
        private int port;
        private string conString;
        object dbcon;

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string ServiceName { get => serviceName; set => serviceName = value; }
        public string SID { get => sID; set => sID = value; }
        public string ServerIP { get => serverIP; set => serverIP = value; }
        public int Port { get => port; set => port = value; }
        public string ConString { get => conString; set => conString = value; }
        public object Dbcon { get => dbcon; set => dbcon = value; }
        public string Module { get; set; }

        public Arguments(string username, string password, string sID, string serverIP, int port,string connString, string module, string serviceName = null)
        {
            this.Username = username;
            this.password = password;
            this.ServiceName = serviceName;
            SID = sID;
            this.ServerIP = serverIP;
            this.Port = port;
            this.ConString = connString;
            this.Module = module;

        }

        public Arguments()
        {
            //empty
        }

    }
}

