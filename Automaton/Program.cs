using System;
using System.IO;
using System.Text;

namespace Automaton
{
    internal class Program
    {
        public static string GetStr(string fileName)
        {
            using (StreamReader stream = new StreamReader(fileName, Encoding.Default))
            {
                StringBuilder s = new StringBuilder(stream.ReadToEnd().ToLower());
                s.Replace("\n", "\\n");
                s.Replace("\r", "\\r");
                s.Replace("\t", "\\t");
                s.Replace(" ", "\\s");
                return s.ToString();
            }
        }

        public static void WriteResultIntoFile(string result)
        {
            using (StreamWriter writer = new StreamWriter("output.txt", true))
            {
                writer.WriteLine(result);
            }
        }

        private static void Main(string[] args)
        {
            AutomatonGenerator gen = new AutomatonGenerator("RegularExpressions.txt");
            foreach (var item in gen._regStorage)
            {
                Console.WriteLine(item);
            }
        }
    }
}