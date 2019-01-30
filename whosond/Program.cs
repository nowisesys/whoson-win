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
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

using WhosOn.Library;
using System.Collections.Generic;

namespace WhosOn.Service
{
    class Program
    {
        private Options options = new Options();

        void Report(Exception exception)
        {
            Console.Error.WriteLine(
                ProgramInfo.ProgramName + ": (" +
                exception.Source + "): " +
                exception.GetType() + ": " +
                exception.Message + "\n" +
                exception.StackTrace
                );
        }

        void Parse(string[] args)
        {
            options.Parse(args);
        }

        public void Process()
        {
            try
            {
                switch (options.GetReason())
                {
                    case Options.Reason.Install:
                        Install();
                        break;
                    case Options.Reason.Remove:
                        Remove();
                        break;
                    case Options.Reason.Service:
                        Service();
                        break;
                }
            }
            catch (Exception exception)
            {
                Report(exception);
            }
        }

        private void Install()
        {
            ServiceParameters parameters = new ServiceParameters(options.Credentials);
            parameters.Add(Assembly.GetExecutingAssembly().Location);

            ManagedInstallerClass.InstallHelper(parameters.ToArray());
            Console.WriteLine("Service installed");
        }

        private void Remove()
        {
            ServiceParameters parameters = new ServiceParameters("/u");
            parameters.Add(Assembly.GetExecutingAssembly().Location);

            ManagedInstallerClass.InstallHelper(parameters.ToArray());
            Console.WriteLine("Service removed");
        }

        private void Service()
        {
            Service service = new Service();
            service.ServiceName = ProgramInfo.Product;
            service.Credentials = options.Credentials;
            ServiceBase.Run(service);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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
