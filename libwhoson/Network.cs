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

            if (this.iface == null)
            {
                throw new NetworkInformationException();    // No usable interface
            }

        }

        public string Computer
        {
            get { return System.Environment.MachineName; }
        }

        public string HwAddress
        {
            get { 
                String h = iface.GetPhysicalAddress().ToString();
                return "" + 
                    h[0] + h[1] + ":" + h[2] + h[3] + ":" +
                    h[4] + h[5] + ":" + h[6] + h[7] + ":" +
                    h[8] + h[9] + ":" + h[10] + h[11];
            }
        }
    }
}
