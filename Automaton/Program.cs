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
                return stream.ReadLine();
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
            Automaton a = new Automaton("ID_Automaton.txt");
            var str = GetStr("Input.txt");
            Console.WriteLine(a.Task1(str));
        }
    }
}