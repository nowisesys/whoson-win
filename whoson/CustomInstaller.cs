using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace WhosOn.Client
{
    [RunInstaller(true)]
    public partial class CustomInstaller : Installer
    {
        public CustomInstaller()
        {
            InitializeComponent();
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

#if DEBUG
            Debugger.Break();       // Invoke debugger
#endif
            // 
            // Process CustomActionData gathered by the setup project.
            // 
            SetupLog("Context Property:");
            foreach (string key in Context.Parameters.Keys)
            {
                SetupLog(string.Format("-> {0} = {1}", key, Context.Parameters[key]));
            }

            string destdir = Context.Parameters["destdir"];
            string soapurl = Context.Parameters["soap_endpoint"];
            string exepath = Context.Parameters["assemblypath"];

            // 
            // Load app.config from the installation directory.
            // 
            Configuration config = ConfigurationManager.OpenExeConfiguration(exepath);
            ServiceModelSectionGroup section = (ServiceModelSectionGroup)config.GetSectionGroup("system.serviceModel");

            // 
            // Update all endpoint addresses with the value determined by setup.
            // 
            foreach (ChannelEndpointElement endpoint in section.Client.Endpoints)
            {
                SetupLog(string.Format("Updating endpoint {0}: URL = {1}", endpoint.Name, soapurl));
                endpoint.Address = new Uri(soapurl);
            }

            // 
            // Write the modified client configuration back to disk.
            // 
            config.Save();
        }
    }
}
