using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.ServiceModel;

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

        void Report(Exception exception)
        {
            if (options.Verbose)
            {
                Console.Error.WriteLine(
                    ProgramInfo.ProgramName + ": (" + 
                    exception.Source + "): " +
                    exception.GetType() + ": " + 
                    exception.Message + "\n" +
                    exception.StackTrace
                    );
            }
            else
            {
                Console.Error.WriteLine(
                    ProgramInfo.ProgramName + ": " + 
                    exception.GetType() + ": " +
                    exception.Message
                    );
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
                Report(exception);
            }
            catch (Exception exception)
            {
                Report(exception);
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
            Program program = new Program();
            try
            {
                program.Parse(args);
                program.Process();
            }
            catch (ArgumentException exception)
            {
                program.Report(exception);
            }
        }
    }
}
