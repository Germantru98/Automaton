using System.Collections.Generic;

namespace Automaton
{
    public class Lexical_Analyzer
    {
        private Dictionary<string, Automaton> _automatonStorage;

        public Lexical_Analyzer()
        {
            _automatonStorage = new Dictionary<string, Automaton>();
            _automatonStorage.Add("kw", new Automaton("kw_Automaton.txt"));
            _automatonStorage.Add("op_eq", new Automaton("op_eq_Automaton.txt"));
            _automatonStorage.Add("op", new Automaton("op_Automaton.txt"));
            _automatonStorage.Add("id", new Automaton("ID_Automaton.txt"));
            _automatonStorage.Add("()", new Automaton("scob_Automaton.txt"));
            _automatonStorage.Add(";", new Automaton(";_Automaton.txt"));
            _automatonStorage.Add("num", new Automaton("num_Automaton.txt"));
            _automatonStorage.Add("str", new Automaton("str_Automaton.txt"));
            _automatonStorage.Add("com", new Automaton("com_Automaton.txt"));
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
                var list = GetAutomatonsForCurrentSymbols(str[i]);
                if (list.Count > 0)
                {
                    var tmpResults = new Dictionary<Automaton, KeyValuePair<bool, int>>();
                    foreach (var item in list)
                    {
                        var tmpToken = item.MaxStr(str, i);
                        if (tmpToken.Key)
                        {
                            tmpResults.Add(item, tmpToken);
                        }
                    }
                    List<Automaton> automatons = new List<Automaton>();
                    foreach (var item in tmpResults.Keys)
                    {
                        automatons.Add(item);
                    }
                    var automatonWithHighestPriority = GetAutomatonWithHighestPriority(automatons);
                    result.Add($"<{automatonWithHighestPriority._automatonName},{str.Substring(i, tmpResults[automatonWithHighestPriority].Value)}>");
                    i += tmpResults[automatonWithHighestPriority].Value;
                }
                else
                {
                    i++;
                }
            }
            return result;
        }

        private List<Automaton> GetAutomatonsForCurrentSymbols(char symbol)
        {
            var result = new List<Automaton>();
            foreach (var item in _automatonStorage.Values)
            {
                if (item.isSymbolInSigma(symbol))
                {
                    result.Add(item);
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
    }
}