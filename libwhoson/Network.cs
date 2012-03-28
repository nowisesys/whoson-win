// WhosOn Client Side Application
// Copyright (C) 2011-2012 Anders Lövgren, Computing Department at BMC, Uppsala University
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
