using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurkysRustBoot.Classes
{
    class RustBootReport
    {
        string command;
        string message;

        public string Command
        {
            get { return command; }
            set { command = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
