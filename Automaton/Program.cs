﻿using System;
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
                StringBuilder s = new StringBuilder(stream.ReadToEnd().ToLower());
                s.Replace("\n", "\\n");
                s.Replace("\r", "\\r");
                s.Replace("\t", "\\t");
                return s.ToString();
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
            Lexical_Analyzer l = new Lexical_Analyzer();
            var test = l.AnalyzeStr(str);
            foreach (var item in test)
            {
                Console.WriteLine(item);
            }
        }
    }
}