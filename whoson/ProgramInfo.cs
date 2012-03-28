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
using System.Reflection;

namespace WhosOn.Client
{
    /// <summary>
    /// Utility class providing static application information.
    /// </summary>
    class ProgramInfo
    {
        /// <summary>
        /// Get command line program name.
        /// </summary>
        public static string ProgramName
        {
            get
            {
                return Environment.GetCommandLineArgs()[0];
            }
        }

        /// <summary>
        /// Get assembly description from manifest.
        /// </summary>
        public static string Description
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyDescriptionAttribute attr = (AssemblyDescriptionAttribute)assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true)[0];
                return attr.Description;
            }
        }

        /// <summary>
        /// Get copyright information from manifest.
        /// </summary>
        public static string Copyright
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyCopyrightAttribute attr = (AssemblyCopyrightAttribute)assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0];
                return attr.Copyright;
            }
        }

        /// <summary>
        /// Get product name from manifest.
        /// </summary>
        public static string Product
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyProductAttribute attr = (AssemblyProductAttribute)assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0];
                return attr.Product;
            }
        }

        /// <summary>
        /// Get version name from manifest.
        /// </summary>
        public static string Version
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyVersionAttribute attr = (AssemblyVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), true)[0];
                return attr.Version;
            }
        }
    }
}
