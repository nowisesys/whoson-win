// WhosOn Client Side Application
// 
// Copyright (C) 2015-2018 Anders Lövgren, BMC-IT, Uppsala University
// Copyright (C) 2018-2019 Anders Lövgren, Nowise Systems
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

using WhosOn.Library.LogonAccountingServiceReference;

namespace WhosOn.Library
{

    class LogonEventConverter
    {
        public static LogonEventProxy Convert(LogonEvent f)
        {
            LogonEventProxy p = new LogonEventProxy();
            p.Domain = f.Domain;
            p.EventID = f.EventID;
            p.Hostname = f.Hostname;
            p.HwAddress = f.HwAddress;
            p.IpAddress = f.IpAddress;
            p.StartTime = f.StartTime;
            p.Username = f.Username;
            p.Workstation = f.Workstation;
            return p;
        }
    }

    /// <summary>
    /// Proxy class for the logon event web service.
    /// </summary>
    public class LogonEventProxy : LogonEvent
    {
        public LogonEventProxy()
        {
        }

        public LogonEventProxy(int eventID)
        {
            this.EventID = eventID;
        }

        public LogonEventProxy(string username, string domain)
            : this(username, domain, null, null)
        {
        }

        public LogonEventProxy(Account account)
            : this(account.UserName, account.Domain, null, null)
        {
        }

        public LogonEventProxy(string username, string domain, string hwaddr, string workstation)
        {
            this.Username = username;
            this.Domain = domain;
            this.HwAddress = hwaddr;
            this.Workstation = workstation;
        }

        /// <summary>
        /// Add logon event and set the EventID.
        /// </summary>
        /// <param name="adapter">The SOAP proxy adapter</param>
        public void Add(LogonEventAdapter adapter)
        {
            if (HwAddress == null)
            {
                Network network = new Network();
                HwAddress = network.HwAddress;
                Workstation = network.Computer;
            }
            EventID = adapter.Add(this);
        }

        /// <summary>
        /// Close logon event represent by this object.
        /// </summary>
        /// <param name="adapter">The SOAP proxy adapter</param>
        public void Close(LogonEventAdapter adapter)
        {
            if (EventID == 0)
            {
                throw new ArgumentException();
            }
            adapter.Close(this);
        }

        /// <summary>
        /// Delete the logon event represent by this object.
        /// </summary>
        /// <param name="adapter">The SOAP proxy adapter</param>
        public void Delete(LogonEventAdapter adapter)
        {
            if (EventID == 0)
            {
                throw new ArgumentException();
            }
            adapter.Delete(this);
        }

        /// <summary>
        /// Find all logon events matching the properties of this object.
        /// </summary>
        /// <param name="adapter">The SOAP proxy adapter</param>
        /// <param name="match">The match preferences.</param>
        /// <returns>Array of LogonEvent objects.</returns>
        public List<LogonEvent> Find(LogonEventAdapter adapter, LogonEventMatch match)
        {
            return adapter.Find(this, match);
        }

    }
}
