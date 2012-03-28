// WhosOn Client Side Application
// Copyright (C) 2011-2012 Anders Lövgren, Computing Department at BMC, Uppsala University
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
