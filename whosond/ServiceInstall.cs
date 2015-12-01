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

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

using WhosOn.Library;
using System.ServiceModel.Description;
using System.Collections.Generic;

namespace WhosOn.Service
{

    [RunInstaller(true)]
    public class ServiceInstall : Installer
    {
        public ServiceInstall()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = ProgramInfo.Title;
            serviceInstaller.Description = ProgramInfo.Description;
            serviceInstaller.ServiceName = ProgramInfo.Product;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }

        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            if (Context.Parameters["HasAuth"] != null)
            {
                Context.Parameters["AssemblyPath"] += "\"";
            }
            if (Context.Parameters["Username"] != null)
            {
                Context.Parameters["AssemblyPath"] += " -U " + Context.Parameters["Username"];
            }
            if (Context.Parameters["Password"] != null)
            {
                Context.Parameters["AssemblyPath"] += " -P " + Context.Parameters["Password"];
            }
            if (Context.Parameters["WinCred"] != null)
            {
                Context.Parameters["AssemblyPath"] += " -W ";
            }

            base.OnBeforeInstall(savedState);
        }
    }

    /// <summary>
    /// Helper class for passing installer util parameters.
    /// </summary>
    class ServiceParameters
    {
        private List<string> args = new List<string>();

        public ServiceParameters()
        {
        }

        public ServiceParameters(string arg)
        {
            args.Add(arg);
        }

        public ServiceParameters(ClientCredentials credentials)
        {
            if (credentials != null)
            {
                args.Add("/HasAuth=1");

                if (credentials.UserName.UserName != null &&
                    credentials.UserName.Password != null)
                {
                    args.Add("/Username=" + credentials.UserName.UserName);
                    args.Add("/Password=" + credentials.UserName.Password);
                }
                else
                {
                    args.Add("/WinCred=1");
                }
            }
        }

        public void Add(string arg)
        {
            args.Add(arg);
        }

        public string[] ToArray()
        {
            return args.ToArray();
        }
    }

}
