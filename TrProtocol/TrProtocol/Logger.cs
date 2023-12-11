using System;
using System.IO;

namespace TrProtocol
{
    public class Logger
    {
        private const string logFilePath = "log.txt";
        public static void Log(object content){
            Log(content.ToString(),true);
        }
        public static void Log(string content){
            Log(content,true);
        }
        public static void Log(string content, bool print)
        {
            content = DateTime.Now.ToString("[yyyy/dd/mm hh:mm:ss]") + content;
            if (print)
                Console.WriteLine(content);
            try
            {
                File.AppendAllLines(logFilePath, new string[] { content });
            }
            catch { }
        }
    }
}