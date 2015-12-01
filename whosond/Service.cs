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
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using WhosOn.Library;
using WhosOn.Library.LogonAccountingServiceReference;
using System.ServiceModel.Description;

namespace WhosOn.Service
{
    public partial class Service : ServiceBase
    {
        private bool debug = false;
        private ClientCredentials credentials;

        public Service()
        {
            InitializeComponent();
            CanHandleSessionChangeEvent = true;
        }

        public ClientCredentials Credentials
        {
            set { credentials = value; }
            get { return credentials; }
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogoff:
                    OnSessionLogoff(changeDescription.SessionId);
                    break;
                case SessionChangeReason.SessionLogon:
                    OnSessionLogon(changeDescription.SessionId);
                    break;
            }

            if (debug)
            {
                EventLog.WriteEntry("OnSessionChange: " + changeDescription.Reason.ToString());
                EventLog.WriteEntry("OnSessionChange: " + GetSessionUser(changeDescription.SessionId));
            }
        }

        protected void OnSessionLogon(int sessionId)
        {
            Account account = new Account(GetSessionUser(sessionId));
            EventLog.WriteEntry(
                string.Format("User {0}\\{1} logged on", account.Domain, account.UserName),
                EventLogEntryType.Information,
                528
                );

            LogonEventProxy proxy = new LogonEventProxy(account);
            LogonEventAdapter adapter = new LogonEventAdapter(credentials);
            proxy.Add(adapter);
        }

        protected void OnSessionLogoff(int sessionId)
        {
            Account account = new Account(GetSessionUser(sessionId));

            EventLog.WriteEntry(
                string.Format("User {0}\\{1} logged off", account.Domain, account.UserName),
                EventLogEntryType.Information,
                528
            );

            LogonEventAdapter adapter = new LogonEventAdapter(credentials);
            LogonEvent record = adapter.Find(account);
            adapter.Close(record.EventID);
        }

        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

        [DllImport("Wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pointer);

        enum WTS_INFO_CLASS
        {
            WTSInitialProgram = 0,
            WTSApplicationName = 1,
            WTSWorkingDirectory = 2,
            WTSOEMId = 3,
            WTSSessionId = 4,
            WTSUserName = 5,
            WTSWinStationName = 6,
            WTSDomainName = 7,
            WTSConnectState = 8,
            WTSClientBuildNumber = 9,
            WTSClientName = 10,
            WTSClientDirectory = 11,
            WTSClientProductId = 12,
            WTSClientHardwareId = 13,
            WTSClientAddress = 14,
            WTSClientDisplay = 15,
            WTSClientProtocolType = 16,
            WTSIdleTime = 17,
            WTSLogonTime = 18,
            WTSIncomingBytes = 19,
            WTSOutgoingBytes = 20,
            WTSIncomingFrames = 21,
            WTSOutgoingFrames = 22,
            WTSClientInfo = 23,
            WTSSessionInfo = 24,
            WTSSessionInfoEx = 25,
            WTSConfigInfo = 26,
            WTSValidationInfo = 27,
            WTSSessionAddressV4 = 28,
            WTSIsRemoteSession = 29
        }

        private static string GetSessionUser(int sessionId)
        {
            IntPtr buffer;
            int strLen;
            var username = "SYSTEM"; // Assume SYSTEM as this will return "\0" below

            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out strLen) && strLen > 1)
            {
                // 
                // Don't need length as these are null terminated strings.
                // 
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
                if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSDomainName, out buffer, out strLen) && strLen > 1)
                {
                    // 
                    // Prepend domain name:
                    // 
                    username = Marshal.PtrToStringAnsi(buffer) + "\\" + username;
                    WTSFreeMemory(buffer);
                }
            }

            return username;
        }

    }
}
