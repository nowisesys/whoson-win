using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Principal;
using System.Threading;

namespace WhosOn.Library
{
    /// <summary>
    /// Handles information about logged on user.
    /// </summary>
    public class Account
    {
        private WindowsIdentity identity;
        static char[] separator = { '\\' };

        public Account()
        {
            identity = WindowsIdentity.GetCurrent();
        }

        /// <summary>
        /// Get username for current logged on user. It don't make sense to use this property
        /// from an Windows service.
        /// </summary>
        public string UserName
        {
            get
            {
                string[] values = identity.Name.Split(separator);
                return values[1];
            }
        }

        /// <summary>
        /// Get domain for current logged on user.
        /// </summary>
        public string Domain
        {
            get
            {
                string[] values = identity.Name.Split(separator);
                return values[0];
            }
        }

        /// <summary>
        /// Get windows identity of logged on user.
        /// </summary>
        public WindowsIdentity Identity
        {
            get { return identity; }
        }
    }
}
