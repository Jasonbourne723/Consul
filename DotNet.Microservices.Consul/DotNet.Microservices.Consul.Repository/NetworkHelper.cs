using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace DotNet.Microservices.Consul.Repository
{
	public class NetworkHelper
	{
		public static string LocalIPAddress
		{
			get
			{
				UnicastIPAddressInformation unicastIPAddressInformation = null;
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface networkInterface in allNetworkInterfaces)
				{
					if (networkInterface.OperationalStatus != OperationalStatus.Up)
					{
						continue;
					}
					IPInterfaceProperties iPProperties = networkInterface.GetIPProperties();
					if (iPProperties.GatewayAddresses.Count == 0)
					{
						continue;
					}
					foreach (UnicastIPAddressInformation unicastAddress in iPProperties.UnicastAddresses)
					{
						if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicastAddress.Address))
						{
							return unicastAddress.Address.ToString();
						}
					}
				}
				if (unicastIPAddressInformation == null)
				{
					return "";
				}
				return unicastIPAddressInformation.Address.ToString();
			}
		}

		public static int GetRandomAvaliablePort(int minPort = 1024, int maxPort = 65535)
		{
			Random random = new Random();
			int num;
			do
			{
				num = random.Next(minPort, maxPort);
			}
			while (IsPortInUsed(num));
			return num;
		}

		private static bool IsPortInUsed(int port)
		{
			IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			if (iPGlobalProperties.GetActiveTcpListeners().Any((IPEndPoint p) => p.Port == port))
			{
				return true;
			}
			if (iPGlobalProperties.GetActiveUdpListeners().Any((IPEndPoint p) => p.Port == port))
			{
				return true;
			}
			if (iPGlobalProperties.GetActiveTcpConnections().Any((TcpConnectionInformation conn) => conn.LocalEndPoint.Port == port))
			{
				return true;
			}
			return false;
		}
	}
}
