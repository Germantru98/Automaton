using System;
using System.Collections.Generic;

namespace Automaton
{
    public class Automaton
    {
        public string _automatonName { get; set; }
        public int _priority { get; set; }
        public List<string> _sigma { get; private set; }
        public Dictionary<State, List<Line>> _delta { get; set; }

        public Automaton()
        {
        }

        public Automaton(string automatonName, int priority, List<string> sigma, Dictionary<State, List<Line>> delta)
        {
            _automatonName = automatonName;
            _priority = priority;
            _sigma = sigma;
            _delta = delta;
        }

        public State GetStartState()
        {
            State result = null;
            foreach (var item in _delta.Keys)
            {
                if (item._stateType == 0)
                    result = item;
            }
            return result;
        }

        private bool isContainsInSigma(string symbol)
        {
            foreach (var item in _sigma)
            {
                if (item == symbol)
                {
                    return true;
                }
            }
            return false;
        }

        private List<string> GetCurSymbolsFromLines(List<Line> lines)
        {
            List<string> result = new List<string>();
            foreach (var item in lines)
            {
                result.Add(item._symbol);
            }
            return result;
        }

        private bool isContainsInCurSignals(string symbol, List<string> symbols)
        {
            bool result = false;
            foreach (var item in symbols)
            {
                if (symbols.Contains(symbol))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private State GetStateBySymbol(State curState, string curSymbol)
        {
            foreach (var item in _delta[curState])
            {
                if (item._symbol == curSymbol)
                {
                    return item._to;
                }
            }
            return null;
        }

        public KeyValuePair<bool, int> MaxStr(string str, int position)
        {
            bool flag = false;
            int maxLength = 0;
            State curState = GetStartState();
            var delta = _delta;
            bool isFinishState;
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
                var curStateValues = delta[curState];
                var curSigma = GetCurSymbolsFromLines(curStateValues);
                if (!isContainsInSigma(str[i].ToString()))
                {
                    return new KeyValuePair<bool, int>(flag, maxLength);
                }
                if (!isContainsInCurSignals(str[i].ToString(), curSigma))
                {
                    return new KeyValuePair<bool, int>(flag, maxLength);
                }
                else
                {
                    curState = GetStateBySymbol(curState, str[i].ToString());
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

        public string Task1(string str)
        {
            var result = new List<string>();
            Console.WriteLine("Последовательность: " + str);
            int i = 0;
            while (i < str.Length)
            {
                var tmp = MaxStr(str, i);
                if (tmp.Key)
                {
                    result.Add($"{str.Substring(i, tmp.Value)}\n");
                    i += tmp.Value;
                }
                else
                {
                    i++;
                }
            }
            return string.Concat(result);
        }

        public void ShowDelta()
        {
            foreach (var state in _delta.Keys)
            {
                Console.WriteLine(state);
                foreach (var line in _delta[state])
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine();
            }
        }

        public void ShowSigma()
        {
            foreach (var item in _sigma)
            {
                Console.Write(item + " ");
            }
        }
    }
}