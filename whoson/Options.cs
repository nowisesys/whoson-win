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
using System.Linq;
using System.ServiceModel.Description;

using WhosOn.Library;
using WhosOn.Library.LogonAccountingServiceReference;

namespace WhosOn.Client
{

    /// <summary>
    /// An helper class for parsing the command line options.
    /// </summary>
    public class Options
    {
        public enum Reason
        {
            Logout, Login, List, Close, Register, Uninstall, Unknown
        }

        public enum Format
        {
            Compact, Human, Tabbed, XML
        }

        public LogonEvent Filter
        {
            get { return filter; }
        }

        public LogonEventMatch Match
        {
            get { return match; }
        }

        public Reason GetReason()
        {
            return reason;
        }

        public Format GetFormat()
        {
            return format;
        }

        public ClientCredentials Credentials
        {
            get { return credentials; }
        }

        public bool Verbose
        {
            get { return verbose; }
        }

        /// <summary>
        /// Called to display application usage.
        /// </summary>
        void Usage()
        {
            Console.WriteLine(ProgramInfo.ProgramName + " - " + ProgramInfo.Description);
            Console.WriteLine();
            Console.WriteLine("Usage: " + ProgramInfo.ProgramName + " [options...]");
            Console.WriteLine("Options:");
            Console.WriteLine("  -i,--logon:       Store logon event.");
            Console.WriteLine("  -o,--logout:      Store logoff event.");
            Console.WriteLine("  -l,--list:        List logon events (see filter and matching)");
            Console.WriteLine("  -F,--close:       Close matching sessions.");
            Console.WriteLine("  -v,--verbose:     Be more verbose.");
            Console.WriteLine("  -h,--help:        Show this help");
            Console.WriteLine("  -V,--version:     Get version info.");
            Console.WriteLine("Filters:");
            Console.WriteLine("     --id=num:      Filter on event ID.");
            Console.WriteLine("     --start=date:  Filter on start date.");
            Console.WriteLine("     --end=date:    Filter on end date.");
            Console.WriteLine("     --comp=name:   Filter on computer name (NetBIOS).");
            Console.WriteLine("     --host=name:   Filter on computer name (FQHN).");
            Console.WriteLine("     --ip=addr:     Filter on IP address.");
            Console.WriteLine("     --hw=addr:     Filter on MAC address.");
            Console.WriteLine("     --user=name:   Filter on username.");
            Console.WriteLine("     --domain=name: Filter on domain name.");
            Console.WriteLine("     --first=num:   Filter on event ID.");
            Console.WriteLine("     --last=num:    Filter on event ID.");
            Console.WriteLine("  -L,--limit=num:   Limit number of rows returned.");
            Console.WriteLine("Matching:");
            Console.WriteLine("  -a,--active:      Match active logons.");
            Console.WriteLine("  -c,--closed:      Match closed logons.");
            Console.WriteLine("     --between:     Match logons between.");
            Console.WriteLine("     --before:      Match logons before.");
            Console.WriteLine("     --after:       Match logons after.");
            Console.WriteLine("  -e,--exact:       Match filter exact.");
            Console.WriteLine("  -t,--this:        This host is implied.");
            Console.WriteLine("Format:");
            Console.WriteLine("  -H,--human:       Output formatted for human reading.");
            Console.WriteLine("  -C,--compact:     Output formatted in compact style.");
            Console.WriteLine("  -T,--tabbed:      Output in tab separated format.");
            Console.WriteLine("  -X,--XML:         Output formatted as XML.");
            Console.WriteLine("Miscellanous:");
            Console.WriteLine("  -r,--register:    Register eventlog source.");
            Console.WriteLine("  -u,--uninstall:   Remove eventlog source.");
            Console.WriteLine("Authentication:");
            Console.WriteLine("  -U username:      Use username for HTTP basic authentication.");
            Console.WriteLine("  -P password:      Use password for HTTP basic authentication.");
            Console.WriteLine("  -W,--Windows:     Use Windows credentials for authentication.");
            Console.WriteLine();
            Console.WriteLine("Notes:");
            Console.WriteLine("1. The --between, --before and --after is limited to datetime (--start/--end) and ID (--first/--last) filtering.");
            Console.WriteLine("2. The --active and --closed option can only be used with exact matching filter options, like --host=xxx");
            Console.WriteLine();
            Console.WriteLine(ProgramInfo.Copyright);
        }

        /// <summary>
        /// Called to display version info.
        /// </summary>
        void Version()
        {
            Console.WriteLine("{0} version {1}", ProgramInfo.ProgramName, ProgramInfo.Version);
        }

        /// <summary>
        /// Parse command line options. Throws ArgumentException for invalid options or
        /// missing required arguments.
        /// </summary>
        /// <param name="args">The command line options.</param>
        /// <exception cref="ArgumentException">ArgumentException</exception>
        public void Parse(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                Environment.Exit(1);
            }
            for (int i = 0; i < args.Length; ++i)
            {
                ProgramOption option = ProgramOption.Create(args[i]);
                switch (option.Key)
                {
                    case "-v":
                    case "--verbose":
                        verbose = true;
                        break;
                    case "-h":
                    case "--help":
                        Usage();
                        Environment.Exit(0);
                        break;
                    case "-V":
                    case "--version":
                        Version();
                        Environment.Exit(0);
                        break;

                    // 
                    // Application reason:
                    // 
                    case "-i":
                    case "--logon":
                        reason = Reason.Login;
                        break;
                    case "-o":
                    case "--logout":
                        reason = Reason.Logout;
                        break;
                    case "-l":
                    case "--list":
                        reason = Reason.List;
                        break;
                    case "-F":
                    case "--close":
                        reason = Reason.Close;
                        break;

                    // 
                    // Filter options:
                    // 
                    case "--id":
                        if (option.HasValue)
                        {
                            filter.EventID = int.Parse(option.Value);
                        }
                        else
                        {
                            filter.EventID = int.Parse(args[++i]);
                        }
                        break;
                    case "--start":
                        if (option.HasValue)
                        {
                            filter.StartTime = DateTime.Parse(option.Value);
                        }
                        else
                        {
                            filter.StartTime = DateTime.Parse(args[++i]);
                        }
                        break;
                    case "--end":
                        if (option.HasValue)
                        {
                            filter.EndTime = DateTime.Parse(option.Value);
                        }
                        else
                        {
                            filter.EndTime = DateTime.Parse(args[++i]);
                        }
                        break;
                    case "--comp":
                        if (option.HasValue)
                        {
                            filter.Workstation = option.Value;
                        }
                        else
                        {
                            filter.Workstation = args[++i];
                        }
                        break;
                    case "--host":
                        if (option.HasValue)
                        {
                            filter.Hostname = option.Value;
                        }
                        else
                        {
                            filter.Hostname = args[++i];
                        }
                        break;
                    case "--ip":
                    case "--ipaddr":
                        if (option.HasValue)
                        {
                            filter.IpAddress = option.Value;
                        }
                        else
                        {
                            filter.IpAddress = args[++i];
                        }
                        break;
                    case "--hw":
                    case "--hwaddr":
                    case "--mac":
                        if (option.HasValue)
                        {
                            filter.HwAddress = option.Value;
                        }
                        else
                        {
                            filter.HwAddress = args[++i];
                        }
                        break;
                    case "--user":
                    case "--username":
                        if (option.HasValue)
                        {
                            filter.Username = option.Value;
                        }
                        else
                        {
                            filter.Username = args[++i];
                        }
                        break;
                    case "--domain":
                        if (option.HasValue)
                        {
                            filter.Domain = option.Value;
                        }
                        else
                        {
                            filter.Domain = args[++i];
                        }
                        break;
                    case "--first":
                        if (option.HasValue)
                        {
                            filter.FirstID = int.Parse(option.Value);
                        }
                        else
                        {
                            filter.FirstID = int.Parse(args[++i]);
                        }
                        break;
                    case "--last":
                        if (option.HasValue)
                        {
                            filter.LastID = int.Parse(option.Value);
                        }
                        else
                        {
                            filter.LastID = int.Parse(args[++i]);
                        }
                        break;
                    case "-L":
                    case "--limit":
                        if (option.HasValue)
                        {
                            filter.Limit = int.Parse(option.Value);
                        }
                        else
                        {
                            filter.Limit = int.Parse(args[++i]);
                        }
                        break;

                    // 
                    // Match options:
                    // 
                    case "-a":
                    case "--active":
                        match = LogonEventMatch.Active;
                        break;
                    case "-c":
                    case "--closed":
                        match = LogonEventMatch.Closed;
                        break;
                    case "--between":
                        match = LogonEventMatch.Between;
                        break;
                    case "--before":
                        match = LogonEventMatch.Before;
                        break;
                    case "--after":
                        match = LogonEventMatch.After;
                        break;
                    case "-e":
                    case "--exact":
                        match = LogonEventMatch.Exact;
                        break;
                    case "-t":
                    case "--this":
                        {
                            Network network = new Network();
                            filter.HwAddress = network.HwAddress;
                            filter.Workstation = network.Computer;
                        }
                        break;

                    // 
                    // Formatting:
                    // 
                    case "-T":
                    case "--tabbed":
                        format = Format.Tabbed;
                        break;
                    case "-C":
                    case "--compact":
                        format = Format.Compact;
                        break;
                    case "-H":
                    case "--human":
                        format = Format.Human;
                        break;
                    case "-X":
                    case "--XML":
                    case "--xml":
                        format = Format.XML;
                        break;

                    // 
                    // Miscellanous:
                    // 
                    case "-r":
                    case "--register":
                        reason = Reason.Register;
                        break;
                    case "-u":
                    case "--uninstall":
                        reason = Reason.Uninstall;
                        break;

                    //
                    // Authentication:
                    // 
                    case "-U":
                        if (credentials == null)
                        {
                            credentials = new ClientCredentials();
                        }
                        if (option.HasValue)
                        {
                            credentials.UserName.UserName = option.Value;
                        }
                        else
                        {
                            credentials.UserName.UserName = args[++i];
                        }
                        break;
                    case "-P":
                        if (credentials == null)
                        {
                                credentials = new ClientCredentials();
                        }
                        if (option.HasValue)
                        {
                            credentials.UserName.Password = option.Value;
                        }
                        else
                        {
                            credentials.UserName.Password = args[++i];
                        }
                        break;
                    case "-W":
                    case "--Windows":
                        if (credentials == null)
                        {
                                credentials = new ClientCredentials();
                        }
                        break;
                    default:
                        throw new ArgumentException("Unknown option '" + args[i] + "'");
                }
            }

            if (reason == Reason.Unknown)
            {
                throw new ArgumentException("Missing -l, -i or -o option, see --help");
            }
            if (filter.LastID != 0)
            {
                if (match != LogonEventMatch.Between)
                {
                    match = LogonEventMatch.Between;
                }
                if (filter.EventID != 0)
                {
                    filter.FirstID = filter.EventID;    // Make ID an alias for FirstID
                }
            }
            if (filter.FirstID != 0 && filter.EventID == 0)
            {
                filter.EventID = filter.FirstID;        // Make FirstID an alias for ID
            }
        }

        private Format format = Format.Compact;
        private Reason reason = Reason.Unknown;
        private LogonEvent filter = new LogonEvent();
        private LogonEventMatch match = LogonEventMatch.Exact;
        private bool verbose = false;
        private ClientCredentials credentials = null;

    }
}
