﻿// WhosOn Client Side Application
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
using System.ServiceModel.Security;
using System.ServiceModel.Description;

using WhosOn.Library.LogonAccountingServiceReference;

namespace WhosOn.Library
{
    public class LogonEventAdapter
    {
        public LogonEventAdapter()
        {
            this.proxy = new LogonAccountingServiceSoapClient();
        }

        public LogonEventAdapter(ClientCredentials credentials)
        {
                this.proxy = new LogonAccountingServiceSoapClient();
                SetClientCredentials(credentials);
        }

        public LogonEventAdapter(LogonAccountingServiceSoapClient proxy)
        {
            this.proxy = proxy;
        }

        public void SetProxy(LogonAccountingServiceSoapClient proxy)
        {
            this.proxy = proxy;
        }

        public LogonAccountingServiceSoapClient GetProxy()
        {
            return this.proxy;
        }

        public int Add(String Username, String Domain, String Workstation, String HwAddress)
        {
            return proxy.CreateLogonEvent(Username, Domain, Workstation, HwAddress);
        }

        public int Add(LogonEvent record)
        {
            return Add(record.Username, record.Domain, record.Workstation, record.HwAddress);
        }

        public void Close(int eventID)
        {
            proxy.CloseLogonEvent(eventID);
        }

        public void Close(LogonEvent record)
        {
            Close(record.EventID);
        }

        public void Delete(int eventID)
        {
            proxy.DeleteLogonEvent(eventID);
        }

        public void Delete(LogonEvent record)
        {
            Delete(record.EventID);
        }

        public List<LogonEvent> Find(LogonEvent filter)
        {
            return Find(filter, LogonEventMatch.Exact);
        }

        public List<LogonEvent> Find(LogonEvent filter, LogonEventMatch match)
        {
            return proxy.FindLogonEvents(filter, match);
        }

        public LogonEvent Find(String Username, String Domain, String Workstation)
        {
            return proxy.FindLogonEvent(Username, Domain, Workstation);
        }

        public LogonEvent Find(Account account)
        {
            Network network = new Network();
            return Find(account.UserName, account.Domain, network.Computer);
        }

        public LogonEvent Find()
        {
            Account account = new Account();
            return Find(account);
        }

        public void SetClientCredentials(ClientCredentials credentials)
        {
            if (credentials != null)
            {
                if (credentials.UserName.UserName != null)
                {
                    UserNamePasswordClientCredential cred = this.proxy.ClientCredentials.UserName;
                    cred.UserName = credentials.UserName.UserName;
                    cred.Password = credentials.UserName.Password;
                }
                else
                {
                    WindowsClientCredential cred = this.proxy.ClientCredentials.Windows;
                    cred.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                }
            }
        }

        private LogonAccountingServiceSoapClient proxy;
    }
}
