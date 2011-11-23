using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;

using WhosOn.Library;
using WhosOn.Library.LogonEventServiceReference;
using WhosOn.Client;

namespace WhosOn.Client
{

    /// <summary>
    /// The main application class.
    /// </summary>
    class Program
    {
        private ProgramOptions options = new ProgramOptions();

        /// <summary>
        /// Constructor.
        /// </summary>
        public Program()
        {
            if (!EventLog.SourceExists(ProgramInfo.Product))
            {
                EventLog.CreateEventSource(ProgramInfo.Product, "Application");
            }
        }

        void Parse(string[] args)
        {
            options.Parse(args);
        }

        void Process()
        {
            try
            {
                switch (options.GetReason())
                {
                    case ProgramOptions.Reason.Login:
                        Login();
                        break;
                    case ProgramOptions.Reason.Logout:
                        Logout();
                        break;
                    case ProgramOptions.Reason.List:
                        List();
                        break;
                }
            }
            catch (Win32Exception exception)
            {
                Console.Error.WriteLine(exception);
            }
        }

        void List()
        {
            LogonEvent[] events = LogonEventProxy.Find(options.Filter, options.Match);
            ProgramOutput output = new ProgramOutput();

            switch (options.GetFormat())
            {
                case ProgramOptions.Format.Compact:
                    output.SetFormat(new OutputFormatCompact());
                    output.Write(events);
                    break;
                case ProgramOptions.Format.Human:
                    output.SetFormat(new OutputFormatHuman());
                    output.Write(events);
                    break;
                case ProgramOptions.Format.Tabbed:
                    output.SetFormat(new OutputFormatTabbed());
                    output.Write(events);
                    break;
                case ProgramOptions.Format.XML:
                    output.SetFormat(new OutputFormatXML());
                    output.Write(events);
                    break;
            }

            if (events.Length > 0)
            {
                foreach (LogonEvent record in events)
                {
                }
            }
        }

        void Login()
        {
            Account account = new Account();

            EventLog.WriteEntry(
                ProgramInfo.Product,
                string.Format("User {0}\\{1} logged on", account.Domain, account.UserName),
                EventLogEntryType.Information,
                528);

            LogonEventProxy proxy = new LogonEventProxy(account);
            proxy.Add();
        }

        void Logout()
        {
            Account account = new Account();

            EventLog.WriteEntry(
                ProgramInfo.Product,
                string.Format("User {0}\\{1} logged off", account.Domain, account.UserName),
                EventLogEntryType.Information,
                528);

            LogonEventProxy proxy = LogonEventProxy.Find(account);
            proxy.Close();
        }

        static void Main(string[] args)
        {
            try
            {
                Program program = new Program();
                program.Parse(args);
                program.Process();
            }
            catch (ArgumentException exception)
            {
                Console.Error.WriteLine(ProgramInfo.ProgramName + ": " + exception.Message);
            }
        }
    }
}
