using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TusiaBot
{
    interface ILogeer
    {
        public static void Access(string message) { }
        public static void Error(Exception ex) { }
    }

    public class Logger : ILogeer
    {
        public static void Access(string message)
        {
            Console.WriteLine(message);
        }

        public static void Error(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ResetColor();
        }
    }
}
