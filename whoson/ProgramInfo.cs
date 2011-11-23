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
