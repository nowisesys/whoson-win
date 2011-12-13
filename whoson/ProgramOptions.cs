using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WhosOn.Library;
using WhosOn.Library.LogonEventServiceReference;

namespace WhosOn.Client
{

    class Option
    {
        private string key;
        private string val;

        public Option(string arg)
        {
            if (arg.Contains('='))
            {
                int pos = arg.IndexOf('=');
                key = arg.Substring(0, pos);
                val = arg.Substring(pos + 1);
            }
            else
            {
                key = arg;
                val = null;
            }
        }

        public bool HasValue
        {
            get { return val != null; }
        }

        public string Value
        {
            get { return val; }
        }

        public string Key
        {
            get { return key; }
        }

        public static Option GetOption(string arg)
        {
            return new Option(arg);
        }
    }

    /// <summary>
    /// An helper class for parsing the command line options.
    /// </summary>
    public class ProgramOptions
    {
        public enum Reason
        {
            Logout, Login, List, Unknown
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
            Console.WriteLine("Matching:");
            Console.WriteLine("  -a,--active:      Match active logons.");
            Console.WriteLine("  -c,--closed:      Match closed logons.");
            Console.WriteLine("     --between:     Match logons between.");
            Console.WriteLine("     --before:      Match logons before.");
            Console.WriteLine("     --after:       Match logons after.");
            Console.WriteLine("  -e,--exact:       Match filter exact.");
            Console.WriteLine("Format:");
            Console.WriteLine("  -H,--human:       Output formatted for human reading.");
            Console.WriteLine("  -C,--compact:     Output formatted in compact style.");
            Console.WriteLine("  -T,--tabbed:      Output in tab separated format.");
            Console.WriteLine("  -X,--XML:         Output formatted as XML.");
            Console.WriteLine();
            Console.WriteLine("The --between, --before and --after can only be used with the --start, --end (and --id) filter.");
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
                Option option = Option.GetOption(args[i]);
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

                    default:
                        throw new ArgumentException("Unknown option '" + args[i] + "'");
                }
            }

            if (reason == Reason.Unknown)
            {
                throw new ArgumentException("Missing -i or -o option, see --help");
            }
        }

        private Format format = Format.Compact;
        private Reason reason = Reason.Unknown;
        private LogonEvent filter = new LogonEvent();
        private LogonEventMatch match = LogonEventMatch.Exact;
        private bool verbose = false;

    }
}
