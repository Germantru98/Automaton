using System.Collections.Generic;
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
                return stream.ReadToEnd();
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

            //·
            string str = GetStr("Input.txt");
            AutomatonGenerator gen = new AutomatonGenerator("RegularExpressions.txt");
            var list = gen.GetAutomatonsByRE();
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer(list);
            foreach (var item in lexicalAnalyzer.Task_2(str))
            {
                System.Console.WriteLine(item);
            }
        }
    }
}