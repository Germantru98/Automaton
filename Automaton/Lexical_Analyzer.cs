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

        public Dictionary<string, string> AnalyzeStr(string str)
        {
            var result = new Dictionary<string, string>();
            int i = 0;
            while (i < str.Length)
            {
                var list = GetAutomatonsForCurrentSymbols(str[i]);
                if (list.Count != 0)
                {
                    var tmpAutomaton = GetAutomatonWithHighestPriority(list);
                    var tmpToken = tmpAutomaton.MaxStr(str, i);
                    if (tmpToken.Key)
                    {
                        result.Add("AutomatonName", $"substr: {str.Substring(i, tmpToken.Value)}\n");
                        i += tmpToken.Value;
                    }
                    else
                    {
                        i++;
                    }
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