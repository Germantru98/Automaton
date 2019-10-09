using System;
using System.Collections.Generic;
using System.IO;

namespace Automaton
{
    internal class Program
    {
        public static KeyValuePair<bool, int> MaxStr(Automaton A, string str, int position)
        {
            bool flag = false;
            int maxLength = 0;
            State curState = A.GetStartState();
            int[,] delta = A._delta;
            for (int i = position; i < str.Length; i++)
            {
                int[] curStateValues = A.GetLineFromMatrixByState(curState);
                int index = A.GetIdFromSigmaByChar(str[i]);
                if (index == 888)
                {
                    Console.WriteLine("error, char not in Sigma");
                    return new KeyValuePair<bool, int>(flag, maxLength);
                }
                else
                {
                    curState = A.GetStateByID(delta[curState._stateID, index]);
                    maxLength++;
                    flag = true;
                }
            }
            return new KeyValuePair<bool, int>(flag, maxLength);
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
            Console.WriteLine(a);
            var str = GetStr("Input.txt");
            Console.WriteLine("Последовательность: "+str);
            Console.WriteLine(MaxStr(a, str, 4));
        }
    }
}