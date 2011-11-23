using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace WhosOn.Library
{
    public class Network
    {
        NetworkInterface iface;

        public Network()
        {
            foreach (NetworkInterface iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (iface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }
                if (iface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    this.iface = iface;
                    break;
                }
            }
        }

        public string Computer
        {
            get { return System.Environment.MachineName; }
        }

        public string HwAddress
        {
            get { return iface.GetPhysicalAddress().ToString(); }
        }
    }
}
