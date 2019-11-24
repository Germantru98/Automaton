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
            string str = "caaac1123";
            AutomatonGenerator gen = new AutomatonGenerator();
            Automaton test = gen.GetAutomatonBySymbol("1");
            Automaton test1 = gen.GetAutomatonBySymbol("a");
            Automaton test2 = gen.GetAutomatonBySpecialSymbol("\\d");
            gen.VerticalBarAction(test, test1);
            gen.Iterration(test);
            gen.ConcatAutomatonsAction(test, test2);
            test.ShowDelta();
            test.ShowSigma();
            List<Automaton> automatons = new List<Automaton>();
            automatons.Add(test);
            LexicalAnalyzer la = new LexicalAnalyzer(automatons);
            System.Console.WriteLine("RESULT");
            foreach (var item in la.Task_2(str))
            {
                System.Console.WriteLine(item);
            }
        }
    }
}