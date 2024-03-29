﻿using System.Collections.Generic;
using System.Text;

namespace Automaton
{
    public class LexicalAnalyzer
    {
        private Dictionary<string, Automaton> _automatonStorage;

        public LexicalAnalyzer(List<Automaton> automatons)
        {
            _automatonStorage = new Dictionary<string, Automaton>();
            foreach (var item in automatons)
            {
                _automatonStorage.Add(item._automatonName, item);
            }
        }

        public void ShowAllAutomatons()
        {
            foreach (var item in _automatonStorage)
            {
                System.Console.WriteLine("Automaton name: {0} with priority: {1}", item.Key, item.Value._priority);
            }
        }

        public List<string> AnalyzeStr(string str)
        {
            var result = new List<string>();
            int i = 0;
            while (i < str.Length)
            {
                var automatonsAndRes = new Dictionary<Automaton, int>();
                foreach (var item in _automatonStorage)
                {
                    var tmpToken = item.Value.MaxStr(str, i);
                    if (tmpToken.Key)
                    {
                        automatonsAndRes.Add(item.Value, tmpToken.Value);
                    }
                }
                if (automatonsAndRes.Count > 0)
                {
                    Dictionary<Automaton, int> tmpTokens = new Dictionary<Automaton, int>();
                    int maxLength = 0;
                    foreach (var item in automatonsAndRes)
                    {
                        if (item.Value > maxLength)
                        {
                            maxLength = item.Value;
                        }
                    }
                    foreach (var item in automatonsAndRes)
                    {
                        if (item.Value == maxLength && !tmpTokens.ContainsKey(item.Key))
                        {
                            tmpTokens.Add(item.Key, item.Value);
                        }
                    }
                    if (tmpTokens.Count > 0)
                    {
                        List<Automaton> automatons = new List<Automaton>();
                        foreach (var item in tmpTokens.Keys)
                        {
                            automatons.Add(item);
                        }
                        var automatonWithHighestPriority = GetAutomatonWithHighestPriority(automatons);
                        var tmpSubstring = str.Substring(i, tmpTokens[automatonWithHighestPriority]);
                        tmpSubstring = StrTransform(tmpSubstring);
                        result.Add($"<{automatonWithHighestPriority._automatonName},{tmpSubstring}>");
                        i += tmpTokens[automatonWithHighestPriority];
                    }
                }
                else
                {
                    i++;
                }
            }
            return result;
        }

        private Automaton GetAutomatonWithHighestPriority(List<Automaton> automatons)
        {
            Automaton result = automatons[0];
            foreach (var item in automatons)
            {
                if (item._priority > result._priority)
                {
                    result = item;
                }
            }
            return result;
        }

        private string StrTransform(string str)
        {
            StringBuilder s = new StringBuilder(str);
            s.Replace("\n", "\\n");
            s.Replace("\r", "\\r");
            s.Replace("\t", "\\t");
            s.Replace(" ", "\\s");
            return s.ToString();
        }

        public List<string> Task_2(string str)
        {
            var tokens = AnalyzeStr(str);
            return tokens;
        }
    }
}