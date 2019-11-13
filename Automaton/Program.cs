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
            var regExp = gen._regStorage[0];
            System.Console.WriteLine(regExp);
            var automaton = gen.GetAutomaton(regExp);
            System.Console.WriteLine(automaton);
            System.Console.WriteLine(automaton.Task1(str));
        }
    }
}