using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noodle;
using Noodle.Engine;

namespace Noodle.Resources
{
    public class RegisterStartup : IStartupTask
    {
        public void Execute()
        {
            Register.JQueryPath = "//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js";
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
