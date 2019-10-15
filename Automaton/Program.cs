using System;
using System.IO;
using System.Text;

namespace Automaton
{
    internal class Program
    {
        public static string MaxStr(Automaton A, string str, int position)
        {
            State curState = A.GetStartState();
            int[,] delta = A._delta;
            string result = string.Empty;
            bool isFinishState = false;
            for (int i = position; i < str.Length; i++)
            {
                int index = A.GetIdFromSigmaByChar(str[i]);
                var tmpData = A.GetLineFromMatrixByState(curState);
                if (index == -1)
                {
                    Console.WriteLine("'{0}'", str[i]);
                    return "Error type 1 Symbol not in unput signals";
                }
                else if (tmpData[index] == 0)
                {
                    return "Error type 2 State can't work with cur symbol";
                }
                else
                {
                    result += str[i];
                    curState = A.GetStateByID(delta[curState._stateID, index]);
                    if (curState._stateType == 2)
                    {
                        isFinishState = true;
                    }
                    else
                    {
                        isFinishState = false;
                    }
                }
            }
            if (isFinishState)
            {
                return "result= " + result;
            }
            else
            {
                return "Error type 3 Not in finish(2) state";
            }
        }

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
            Automaton a = new Automaton("Automaton_1.txt");
            var str = GetStr("Input.txt");
            Console.WriteLine("Последовательность: " + str);
            for (int i = 0; i < str.Length; i++)
            {
                //WriteResultIntoFile(MaxStr(a, str, i));
                Console.WriteLine(MaxStr(a, str, i));
            }
        }
    }
}