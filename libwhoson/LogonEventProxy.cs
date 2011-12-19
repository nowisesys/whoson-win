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
        public void Add()
        {
            if (HwAddress == null)
            {
                Network network = new Network();
                HwAddress = network.HwAddress;
                Workstation = network.Computer;
            }
            using (LogonAccountingServiceSoapClient client = new LogonAccountingServiceSoapClient())
            {
                EventID = client.CreateLogonEvent(Username, Domain, Workstation, HwAddress);
            }
        }

        /// <summary>
        /// Close logon event represent by this object.
        /// </summary>
        public void Close()
        {
            if (EventID == 0)
            {
                throw new ArgumentException();
            }
            using (LogonAccountingServiceSoapClient client = new LogonAccountingServiceSoapClient())
            {
                client.CloseLogonEvent(EventID);
            }
        }

        /// <summary>
        /// Delete the logon event represent by this object.
        /// </summary>
        public void Delete()
        {
            if (EventID == 0)
            {
                throw new ArgumentException();
            }
            using (LogonAccountingServiceSoapClient client = new LogonAccountingServiceSoapClient())
            {
                client.DeleteLogonEvent(EventID);
            }
        }

        /// <summary>
        /// Find all logon events matching the properties of this object.
        /// </summary>
        /// <returns>Array of LogonEvent objects.</returns>
        public List<LogonEvent> Find()
        {
            return Find(this);
        }

        /// <summary>
        /// Find all logon events matching the given filter.
        /// </summary>
        /// <param name="filter">The logon event filter.</param>
        /// <returns>Array of LogonEvent objects.</returns>
        public static List<LogonEvent> Find(LogonEvent filter)
        {
            return Find(filter, LogonEventMatch.Exact);
        }

        /// <summary>
        /// Find all logon events matching the given filter and match preferences.
        /// </summary>
        /// <param name="filter">The logon event filter.</param>
        /// <param name="match">The match preferences.</param>
        /// <returns>Array of LogonEvent objects.</returns>
        public static List<LogonEvent> Find(LogonEvent filter, LogonEventMatch match)
        {
            using (LogonAccountingServiceSoapClient client = new LogonAccountingServiceSoapClient())
            {
                return client.FindLogonEvents(filter, match);
            }
        }

        /// <summary>
        /// Find the logon event proxy matching supplied username, domain and computer name (the NetBIOS name).
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="domain">The logon domain.</param>
        /// <param name="computer">The computer name.</param>
        /// <returns>An LogonEventProxy object.</returns>
        public static LogonEventProxy Find(string username, string domain, string computer)
        {
            using (LogonAccountingServiceSoapClient client = new LogonAccountingServiceSoapClient())
            {
                LogonEvent source = client.FindLogonEvent(username, domain, computer);
                return LogonEventConverter.Convert(source);
            }
        }

        /// <summary>
        /// Find the logon event proxy matching the supplied account. The current computer name
        /// is implicit user as the logon computer in the lookup.
        /// </summary>
        /// <param name="account">The account object.</param>
        /// <returns>An LogonEventProxy object.</returns>
        public static LogonEventProxy Find(Account account)
        {
            Network network = new Network();
            return Find(account.UserName, account.Domain, network.Computer);
        }

    }
}
