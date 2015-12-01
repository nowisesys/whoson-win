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

using System.Linq;

namespace WhosOn.Library
{
    public class ProgramOption
    {
        private string key;
        private string val;

        public ProgramOption(string arg)
        {
            if (arg.Contains('='))
            {
                int pos = arg.IndexOf('=');
                key = arg.Substring(0, pos);
                val = arg.Substring(pos + 1);
            }
            else
            {
                key = arg;
                val = null;
            }
        }

        public bool HasValue
        {
            get { return val != null; }
        }

        public string Value
        {
            get { return val; }
        }

        public string Key
        {
            get { return key; }
        }

        public static ProgramOption Create(string arg)
        {
            return new ProgramOption(arg);
        }
    }
}
