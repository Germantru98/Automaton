using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            bool isFinishState = false;
            if (curState._stateType == 2)
            {
                isFinishState = true;
            }
            else
            {
                isFinishState = false;
            }
            for (int i = position; i < str.Length; i++)
            {
                int[] curStateValues = A.GetLineFromMatrixByState(curState);
                int index = A.GetIdFromSigmaByChar(str[i]);
                if (index == -1)
                {
                    return new KeyValuePair<bool, int>(flag, maxLength);
                }
                else if (curStateValues[index] == 0)
                {
                    return new KeyValuePair<bool, int>(flag, maxLength);
                }
                else
                {
                    curState = A.GetStateByID(delta[curState._stateID, index]);
                    maxLength++;
                    flag = true;
                    if (curState._stateType == 2)
                    {
                        isFinishState = true;
                    }
                    else
                    {
                        flag = false;
                        isFinishState = false;
                    }
                }
            }
            if (isFinishState)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            return new KeyValuePair<bool, int>(flag, maxLength);
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
            int i = 0;
            while (i < str.Length)
            {
                var tmp = MaxStr(a, str, i);
                if (tmp.Key)
                {
                    Console.WriteLine("token: {0} result: {1}", tmp, str.Substring(i, tmp.Value));
                    i += tmp.Value;
                }
                else
                {
                    i++;
                }
            }
        }
    }
}