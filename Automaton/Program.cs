using System;
using System.IO;

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
                if (index == -1)
                {
                    result = "error";
                    return result;
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
                return result;
            }
            else
            {
                result = "error";
                return result;
            }
        }

        public static string GetStr(string fileName)
        {
            using (StreamReader stream = new StreamReader(fileName))
            {
                return stream.ReadLine().Trim();
            }
        }

        private static void Main(string[] args)
        {
            Automaton a = new Automaton("Automaton_1.txt");
            var str = GetStr("Input.txt");
            Console.WriteLine("Последовательность: " + str);
            for (int i = 0; i < str.Length; i++)
            {
                Console.WriteLine(MaxStr(a, str, i));
            }
        }
    }
}