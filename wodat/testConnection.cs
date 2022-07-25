using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace wodat
{
	public class testConnection
	{
		public Arguments cArgs;
		public testConnection(Arguments nArgs)
		{
			this.cArgs = nArgs;

		}

		public bool testConn()
        {
            var response = "";
            bool success = false;
			OracleDatabase nDB = new OracleDatabase(cArgs);
			response = (String)nDB.connectDB();
			if (response.Contains("rue"))
			{	return success;}
			else { return success; }
        }
	


	}
}

