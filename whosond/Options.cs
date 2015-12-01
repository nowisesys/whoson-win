// WhosOn Client Side Application
// Copyright (C) 2015 Anders Lövgren, Computing Department at BMC, Uppsala University
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

namespace WhosOn.Service
{

    /// <summary>
    /// An helper class for parsing the command line options.
    /// </summary>
    public class Options
    {
        public enum Reason
        {
            Install, Remove, Service
        }

        public Reason GetReason()
        {
            return reason;
        }

        public ClientCredentials Credentials
        {
            get { return credentials; }
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
            Console.WriteLine("  -h,--help:        Show this help");
            Console.WriteLine("  -V,--version:     Get version info.");
            Console.WriteLine("Miscellanous:");
            Console.WriteLine("  -i,--install:     Install service.");
            Console.WriteLine("  -r,--remove:      Remove service.");
            Console.WriteLine("Authentication:");
            Console.WriteLine("  -U username:      Use username for HTTP basic authentication.");
            Console.WriteLine("  -P password:      Use password for HTTP basic authentication.");
            Console.WriteLine("  -W,--Windows:     Use Windows credentials for authentication.");
            Console.WriteLine();
            Console.WriteLine("Notes:");
            Console.WriteLine("1. Options for authentication is used as service startup parameters.");
            Console.WriteLine("   Example: {0} -i -U username -P secret", ProgramInfo.ProgramName);
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
            for (int i = 0; i < args.Length; ++i)
            {
                ProgramOption option = ProgramOption.Create(args[i]);
                switch (option.Key)
                {
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
                    // Miscellanous:
                    // 
                    case "-i":
                    case "--install":
                        reason = Reason.Install;
                        break;
                    case "-r":
                    case "--remove":
                        reason = Reason.Remove;
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
        }

        private Reason reason = Reason.Service;
        private LogonEvent filter = new LogonEvent();
        private ClientCredentials credentials = null;

    }
}
