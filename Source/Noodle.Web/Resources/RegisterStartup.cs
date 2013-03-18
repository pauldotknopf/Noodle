using Noodle.Resources;

namespace Noodle.Web.Resources
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
