﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.ServiceModel;

using WhosOn.Library;
using WhosOn.Library.LogonAccountingServiceReference;
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
                    case ProgramOptions.Reason.Close:
                        Close();
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

        /// <summary>
        /// Close all records matching the filter options.
        /// </summary>
        void Close()
        {
            LogonEventAdapter adapter = new LogonEventAdapter();
            List<LogonEvent> events = adapter.Find(options.Filter, options.Match);

            foreach (LogonEvent record in events)
            {
                adapter.Close(record);
            }
        }

        /// <summary>
        /// List all records matching the filter options.
        /// </summary>
        void List()
        {
            LogonEventAdapter adapter = new LogonEventAdapter();
            List<LogonEvent> events = adapter.Find(options.Filter, options.Match);
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

        /// <summary>
        /// Records an login event.
        /// </summary>
        void Login()
        {
            Account account = new Account();

            EventLog.WriteEntry(
                ProgramInfo.Product,
                string.Format("User {0}\\{1} logged on", account.Domain, account.UserName),
                EventLogEntryType.Information,
                528);

            LogonEventProxy proxy = new LogonEventProxy(account);
            LogonEventAdapter adapter = new LogonEventAdapter();
            proxy.Add(adapter);
        }

        /// <summary>
        /// Records an logout event. The event ID is remotelly queried using the logged on
        /// user and domain as search preferences. The current workstation is detected
        /// automatic.
        /// </summary>
        void Logout()
        {
            Account account = new Account();

            EventLog.WriteEntry(
                ProgramInfo.Product,
                string.Format("User {0}\\{1} logged off", account.Domain, account.UserName),
                EventLogEntryType.Information,
                528);

            LogonEventAdapter adapter = new LogonEventAdapter();
            LogonEvent record = adapter.Find(account);
            adapter.Close(record.EventID);
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
