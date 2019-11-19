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
            var str = GetStr("Input.txt");
            AutomatonGenerator gen = new AutomatonGenerator("RegularExpressions.txt");
            List<Automaton> automatons = new List<Automaton>();
            foreach (var item in gen._regStorage)
            {
                automatons.Add(gen.GetAutomaton(item));
            }
            Lexical_Analyzer lexical_Analyzer = new Lexical_Analyzer(automatons);
            var result = lexical_Analyzer.Task_2(str);
            System.Console.WriteLine("RESULT: ");
            foreach (var item in result)
            {
                System.Console.WriteLine(item);
            }
        }
    }
}