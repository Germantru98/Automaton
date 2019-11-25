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
            string str = "{aishdoad}asdad";
            AutomatonGenerator gen = new AutomatonGenerator();
            RegularExpression re = new RegularExpression("test", 0, "\\{·\\w*·\\}");

            var automaton = gen.CreateAutomatonByRE(re);
            automaton.ShowDelta();
            List<Automaton> list = new List<Automaton>();
            list.Add(automaton);
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer(list);
            foreach (var item in lexicalAnalyzer.Task_2(str))
            {
                System.Console.WriteLine(item);
            }
        }
    }
}