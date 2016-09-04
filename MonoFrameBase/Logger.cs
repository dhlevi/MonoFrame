using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFrame
{
    public class Logger
    {
        private static volatile Logger instance;
        private static object syncRoot = new Object();

        public Logger()
        {
            // set writing path, IO
        }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new Logger();
                    }
                }
                return instance;
            }
        }

        public static void Log(string message)
        {

        }

        public static void Log(Exception error)
        {

        }
    }
}
