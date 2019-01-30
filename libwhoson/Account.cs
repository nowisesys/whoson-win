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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Principal;
using System.Threading;
using System.Security;

namespace WhosOn.Library
{
    /// <summary>
    /// Handles information about logged on user.
    /// </summary>
    public class Account
    {
        private WindowsIdentity identity = null;

        private string username;
        private string domain;

        static char[] separators = { '\\', '@' };

        /// <summary>
        /// Create account object using logged in user context.
        /// </summary>
        public Account()
        {
            identity = WindowsIdentity.GetCurrent();
            SetUser(identity.Name);
        }

        /// <summary>
        /// Create account object using passed used principal name (UPN).
        /// </summary>
        /// <param name="principal">The format is either "user@domian" or "domain\\user".</param>
        public Account(string principal)
        {
            SetUser(principal);
        }

        /// <summary>
        /// Create account object using passed user and domain.
        /// </summary>
        /// <param name="user">The user name.</param>
        /// <param name="domain">The domain name.</param>
        public Account(string user, string domain)
        {
            SetUser(user + '@' + domain);
        }

        /// <summary>
        /// Get user name.
        /// </summary>
        public string UserName
        {
            get
            {
                return username;
            }
        }

        /// <summary>
        /// Get logon domain.
        /// </summary>
        public string Domain
        {
            get
            {
                return domain;
            }
        }

        /// <summary>
        /// Get windows identity. Might be null for unresolved windows identities
        /// when using non-default constructors.
        /// </summary>
        public WindowsIdentity Identity
        {
            get { return identity; }
        }

        private void SetUser(string user)
        {
            string[] values;

            if (user.Contains('\\'))
            {
                values = user.Split(separators);
                username = values[1];
                domain = values[0];
            }
            else if (user.Contains('@'))
            {
                values = user.Split(separators);
                username = values[0];
                domain = values[1];
            }
            else
            {
                username = user;
                domain = "";
            }

            if (identity == null && domain.Length > 1)
            {
                try
                {
                    identity = new WindowsIdentity(username + '@' + domain);
                }
                catch(SecurityException)
                {
                }
            }
        }

    }
}
