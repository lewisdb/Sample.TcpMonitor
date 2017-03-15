using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.TcpMonitor
{
    /// <summary>
    /// class for handling exceptions
    /// </summary>
    public static class Logger
    {
        public static void WriteMessage(string errMsg)
        {
            try
            {
                //TODO: implement method for handling exception
                //for now
                Console.WriteLine(errMsg);
            }
            catch (Exception ex)
            {
                //for now:
                Console.WriteLine(ex.Message);
            }
        }
    }
}
