using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Expanze.Utils
{
    public class Logger
    {
        private static Logger instance = null;

        public static Logger Inst()
        {
            if (instance == null)
            {
                instance = new Logger();
            }

            return instance;
        }

        public void Log(String src, String message)
        {
            TextWriter tw = new StreamWriter(src);
            tw.WriteLine(message);
            tw.Close();
        }
    }
}
