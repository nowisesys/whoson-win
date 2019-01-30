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
using System.Text;

using WhosOn.Library.LogonAccountingServiceReference;

namespace WhosOn.Client
{
    public interface IOutputFormat
    {
        void WriteHeader();
        void WriteFooter(List<LogonEvent> records);
        void WriteRecord(LogonEvent record);
    }

    class OutputFormatHuman : IOutputFormat
    {
        public static string Separator = "-----------------------------------------------------";

        public void WriteHeader()
        {
            // ignore
        }

        public void WriteFooter(List<LogonEvent> records)
        {
            Console.WriteLine(Separator);
            Console.WriteLine("Summary: {0} records in output", records.Count);
        }

        public void WriteRecord(LogonEvent record)
        {
            Console.WriteLine("User: {0}\\{1} (Event ID={2})", record.Username, record.Domain, record.EventID);
            Console.WriteLine(Separator);
            Console.WriteLine("\t  Computer: {0} (NetBIOS Name: {1})", record.Hostname, record.Workstation);
            Console.WriteLine("\tIP-address: {0} (MAC-address: {1})", record.IpAddress, record.HwAddress);
            if (record.EndTime == DateTime.MinValue)
            {
                Console.WriteLine("\t   Session: {0} -> [(still logged on)]", record.StartTime);
            }
            else
            {
                Console.WriteLine("\t   Session: {0} -> {1}", record.StartTime, record.EndTime);
            }
            Console.WriteLine();
        }
    }

    class OutputFormatTabbed : IOutputFormat
    {
        public void WriteHeader()
        {
            Console.WriteLine("Event ID:\tUsername:\tDomain:\tMAC:\tIP-address\tHostname:\tWorkstation (NetBIOS):\tStart:\tEnd:");
        }
        
        public void WriteFooter(List<LogonEvent> records)
        {
            // ignore
        }

        public void WriteRecord(LogonEvent record)
        {
            Console.WriteLine(
                record.EventID + "\t" +
                record.Username + "\t" +
                record.Domain + "\t" +
                record.HwAddress + "\t" +
                record.IpAddress + "\t" +
                record.Hostname + "\t" +
                record.Workstation + "\t" +
                record.StartTime + "\t" +
                record.EndTime
                );
        }
    }

    class OutputFormatCompact : IOutputFormat
    {
        public void WriteHeader()
        {
            Console.WriteLine("EventID: Username: Domain: MAC: IP-address Hostname: Workstation: Start: End:");
        }

        public void WriteFooter(List<LogonEvent> records)
        {
            // ignore
        }

        public void WriteRecord(LogonEvent record)
        {
            Console.WriteLine(
                record.EventID + " " +
                record.Username + " " +
                record.Domain + " " +
                record.HwAddress + " " +
                record.IpAddress + " " +
                record.Hostname + " " +
                record.Workstation + " " +
                record.StartTime + " " +
                record.EndTime
                );
        }
    }

    class OutputFormatXML : IOutputFormat
    {
        public void WriteHeader()
        {
            Console.WriteLine("<?xml encoding='utf8' version='1.0' ?>");
            Console.WriteLine("<LogonEvents>");
        }

        public void WriteFooter(List<LogonEvent> records)
        {
            Console.WriteLine("</LogonEvents>");
        }

        public void WriteRecord(LogonEvent record)
        {
            Console.WriteLine("  <LogonEvent ID={0}>", record.EventID);
            Console.WriteLine("    <Logon>");
            Console.WriteLine("      <Username>{0}</Username>", record.Username);
            Console.WriteLine("      <Domain>{0}</Domain>", record.Domain);
            Console.WriteLine("    </Logon>");
            Console.WriteLine("    <Computer>");
            Console.WriteLine("      <HwAddress>{0}</HwAddress>", record.HwAddress);
            Console.WriteLine("      <IpAddress>{0}</IpAddress>", record.IpAddress);
            Console.WriteLine("      <Hostname>{0}</Hostname>", record.Hostname);
            Console.WriteLine("      <Workstation>{0}</Workstation>", record.Workstation);
            Console.WriteLine("    </Computer>");
            Console.WriteLine("    <Session>");
            Console.WriteLine("      <StartTime>{0}</StartTime>", record.StartTime);
            Console.WriteLine("      <EndTime>{0}</EndTime>", record.EndTime);
            Console.WriteLine("    </Session>");
            Console.WriteLine("  </LogonEvent>");
        }
    }

    public class ProgramOutput
    {
        public void SetFormat(IOutputFormat format)
        {
            this.format = format;
        }

        public void Write(List<LogonEvent> records)
        {
            if (records.Count > 0)
            {
                format.WriteHeader();
                foreach (LogonEvent record in records)
                {
                    format.WriteRecord(record);
                }
                format.WriteFooter(records);
            }
        }

        private IOutputFormat format;
    }
}
