using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace WhosOn.Client.Installer
{

    /// <summary>
    /// Maintains the installer context parameters. Both builtin and custom.
    /// </summary>
    class InstallerParams
    {
        InstallContext context;

        class Keys
        {
            public class SoapEndpoint
            {
                public const string User = "SoapEndpointUser";
                public const string Exec = "SoapEndpointExec";
            }

            public const string AssemblyPath = "AssemblyPath";
            public const string InstallDir = "DestDir";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The installer context</param>
        public InstallerParams(InstallContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="state">The saved state parameters.</param>
        public InstallerParams(IDictionary state)
        {
            this.context = new InstallContext();
            foreach (string key in state.Keys)
            {
                if (typeof(string) == state[key].GetType())
                {
                    this.context.Parameters.Add(key, (string)state[key]);
                }
            }
        }

        /// <summary>
        /// Get collection of all installer context parameters.
        /// </summary>
        /// <returns></returns>
        public System.Collections.Specialized.StringDictionary GetCollection()
        {
            return context.Parameters;
        }

        /// <summary>
        /// Check if the given key is found in the installer context.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <returns>True if the parameter exist and has an non-null value.</returns>
        private bool HasValue(string name)
        {
            return context.Parameters.ContainsKey(name) && context.Parameters[name].Length > 0;
        }

        /// <summary>
        /// Get value of named context parameter.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <returns>The context parameter value.</returns>
        public string GetValue(string name)
        {
            if (HasValue(name))
            {
                return context.Parameters[name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get SOAP service endpoint address. This function favour a value passed on
        /// the command line over a property page value (installer GUI).
        /// </summary>
        /// <returns></returns>
        public string GetSoapEndpoint()
        {
            if (HasValue(Keys.SoapEndpoint.Exec))
            {
                return context.Parameters[Keys.SoapEndpoint.Exec];
            }
            else
            {
                return context.Parameters[Keys.SoapEndpoint.User];
            }
        }

        /// <summary>
        /// Get the installed assembly absolute path.
        /// </summary>
        public string AssemblyPath
        {
            get { return context.Parameters[Keys.AssemblyPath]; }
        }

        /// <summary>
        /// Get the installation directory.
        /// </summary>
        public string InstallDir
        {
            get { return context.Parameters[Keys.InstallDir]; }
        }

        /// <summary>
        /// Save context parameters in state dictionary.
        /// </summary>
        /// <param name="state">The state dictionary.</param>
        public void Save(IDictionary state)
        {
            foreach (string key in context.Parameters.Keys)
            {
                state[key] = context.Parameters[key];
            }
        }
    }

    [RunInstaller(true)]
    public partial class CustomInstaller : System.Configuration.Install.Installer
    {

        public CustomInstaller()
        {
            InitializeComponent();
#if DEBUG
            Debugger.Break();       // Invoke debugger
#endif
        }

        private void SetupLog(string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#endif
            Context.LogMessage(message);
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            // 
            // Create context parameter object.
            // 
            InstallerParams options = new InstallerParams(Context);
            options.Save(stateSaver);

            // 
            // Load app.config from the installation directory.
            // 
            Configuration config = ConfigurationManager.OpenExeConfiguration(options.AssemblyPath);
            ServiceModelSectionGroup section = (ServiceModelSectionGroup)config.GetSectionGroup("system.serviceModel");

            // 
            // Update all endpoint addresses with the value determined by setup.
            // 
            foreach (ChannelEndpointElement endpoint in section.Client.Endpoints)
            {
                SetupLog(string.Format("Updating endpoint address for {0}: URL = {1}", endpoint.Name, options.GetSoapEndpoint()));
                endpoint.Address = new Uri(options.GetSoapEndpoint());
            }

            // 
            // Write the modified client configuration back to disk.
            // 
            config.Save();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            InstallerParams options = new InstallerParams(savedState);
        }
    }
}
