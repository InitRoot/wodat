using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace wodat
{
    public class TNS
    {
		public Arguments cArgs;
		public TNS(Arguments nArgs)
		{
			this.cArgs = nArgs;

		}

		private static readonly byte[] Sender = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
        string versionPacket = "";
		private readonly byte[] HeaderDimension = new byte[24];
		private byte[] CommandCode;
		private readonly byte[] Receiver = Sender;
		private readonly byte[] Error = new byte[] { 0 };
		private readonly byte[] DataDimension = new byte[] { 0 };

		private void sendTCP()
		{





			try
			{

				CommandCode = new byte[4] { 0x35, 0x0, 0x0, 0x4 };
				using (TcpClient tcpClient = new TcpClient(cArgs.ServerIP, cArgs.Port))
				{
					NetworkStream networkStream = tcpClient.GetStream();

					byte[] bytesTosend = HeaderDimension.Concat(CommandCode)
														.Concat(Sender)
														.Concat(Receiver)
														.Concat(Error)
														.Concat(DataDimension).ToArray();

					networkStream.Write(bytesTosend, 0, bytesTosend.Length);
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

		}


	}




}
