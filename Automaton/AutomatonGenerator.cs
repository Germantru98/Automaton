using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automaton
{
    internal class AutomatonGenerator
    {
        public List<RegularExpression> _regStorage { get; private set; }
        public Dictionary<string, List<char>> _specialSymbols { get; private set; }

        public AutomatonGenerator(string fileName)
        {
            using (StreamReader stream = new StreamReader(fileName))
            {
                _regStorage = new List<RegularExpression>();
                var lineCount = int.Parse(stream.ReadLine());
                for (int i = 0; i < lineCount; i++)
                {
                    var tmpLine = stream.ReadLine().Split();
                    _regStorage.Add(new RegularExpression(tmpLine[0], int.Parse(tmpLine[1]), tmpLine[2]));
                }
                setSpecialSymbols();
            }
        }

        private List<char> getChars(string fileName, int modifier)//0 - без модификатора, 1 - Upper, 2 - Lower
        {
            using (StreamReader stream = new StreamReader(fileName))
            {
                List<char> result = new List<char>();
                if (modifier == 0)
                {
                    var str = stream.ReadToEnd();
                    foreach (var item in str)
                    {
                        result.Add(item);
                    }
                }
                else if (modifier == 1)
                {
                    var str = stream.ReadToEnd().ToUpper();
                    foreach (var item in str)
                    {
                        result.Add(item);
                    }
                }
                else
                {
                    var str = stream.ReadToEnd().ToLower();
                    foreach (var item in str)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
        }

        private List<char> getWS()
        {
            char[] chars = { '\n', ' ', '\t', '\r' };
            return chars.ToList();
        }

        private void setSpecialSymbols()
        {
            _specialSymbols = new Dictionary<string, List<char>>();
            _specialSymbols.Add("\\W", getChars("Alphabet.txt", 1));
            _specialSymbols.Add("\\w", getChars("Alphabet.txt", 2));
            _specialSymbols.Add("\\d", getChars("Digits.txt", 0));
            _specialSymbols.Add("\\s", getWS());
            _specialSymbols.Add("\\_", getChars("_.txt", 0));
        }

        private List<string> SplitByBrackets(string re)
        {
            List<string> result = new List<string>();
            int i = 0;
            while (i < re.Length)
            {
                var tmpStr = "";
                if (re[i] == '(')
                {
                    i++;
                    while (re[i] != ')')
                    {
                        tmpStr += re[i];
                        i++;
                    }
                }
                if (re[i] == '*')
                {
                    tmpStr += "*";
                }
                i++;
                result.Add(tmpStr);
            }
            return result; ;
        }

        private List<int> SearchStarInStr(List<string> strings)
        {
            List<int> result = new List<int>();
            int counter = 0;
            foreach (var item in strings)
            {
                if (item == "*")
                {
                    result.Add(counter);
                }
                counter++;
            }
            return result;
        }

        private List<string> SplitByVerticalBar(string str)
        {
            return str.Split('|').ToList();
        }

        private List<string> SplitByBackslash(string str)
        {
            List<string> result = new List<string>();
            foreach (var item in str)
            {
                if (item != '\\')
                {
                    result.Add(item.ToString());
                }
            }
            return result;
        }

        private int CountNumberOfStates(string str)//Подсчет кол-ва состояний для 1-ой скобки
        {
            int counter = 0;
            foreach (var item in str)
            {
                if (item != '|' && item != '\\' && item != '*')
                {
                    counter++;
                }
            }
            return counter;
        }

        private int CountNumberOfInputSignals(string str)
        {
            int counter = 0;
            var tmpSymbols = new List<string>();
            foreach (var item in str)
            {
                if (item != '|' && item != '\\' && item != '*')
                {
                    if (!tmpSymbols.Contains(item.ToString()))
                    {
                        tmpSymbols.Add(item.ToString());
                        counter += _specialSymbols['\\' + item.ToString()].Count;
                    }
                }
            }
            return counter;
        }//Подсчет кол-ва входных сигналов для всей регулярки

        private List<State> CreateStatesForAutomaton(List<string> parts)
        {
            List<State> states = new List<State>();
            states.Add(new State(0, 0, "S0"));
            int stateID = 1;
            foreach (var item in parts)
            {
                if (item != "*")
                {
                    var splitByVerBar = SplitByVerticalBar(item);
                    foreach (var i in splitByVerBar)
                    {
                        var symbols = SplitByBackslash(i);
                        for (int s = 0; s < symbols.Count; s++)
                        {
                            int statePos = 1;
                            if (s == symbols.Count - 1)
                            {
                                statePos = 2;
                            }
                            states.Add(new State(stateID, statePos, $"S{stateID}"));
                            stateID++;
                        }
                    }
                }
            }
            return states;
        }

        private Dictionary<int, char> CreateSigma(string str)
        {
            int counter = 0;
            Dictionary<int, char> result = new Dictionary<int, char>();
            List<char> chars = new List<char>();
            foreach (var i in str)
            {
                if (i != '\\' && i != '*' && i != '|' && !chars.Contains(i))
                {
                    chars.Add(i);
                }
            }
            foreach (var item in chars)
            {
                string key = $"\\{item}";
                var tmpSybols = _specialSymbols[key];
                foreach (var j in tmpSybols)
                {
                    result.Add(counter, j);
                    counter++;
                }
            }
            return result;
        }

        private List<char> GetControlCharsFromCurPart(string str)
        {
            List<char> result = new List<char>();
            foreach (var item in str)
            {
                if (item != '\\' && item != '|')
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private int GetIdByChar(char c, Dictionary<int, char> sigma)
        {
            int result = -1;
            foreach (var item in sigma)
            {
                if (c == item.Value)
                {
                    result = item.Key;
                }
            }
            return result;
        }

        private int[,] GetDelta(int n, int m, List<State> states, Dictionary<int, char> sigma, List<string> regExpParts)
        {
            int[,] result = new int[n, m];
            int pointer = 1;
            List<int> iterationsPos = SearchStarInStr(regExpParts);
            for (int i = 0; i < regExpParts.Count; i++)
            {
                var StartState = states[0];
                if (regExpParts[i] != "*")
                {
                    if (!iterationsPos.Contains(i + 1))
                    {
                        var countOfStateInCurParts = CountNumberOfStates(regExpParts[i]);
                        var tmpStates = new List<State>();
                        for (int j = pointer; j < states.Count; j++)
                        {
                            if (pointer < states.Count)
                            {
                                tmpStates.Add(states[j]);
                                pointer++;
                                countOfStateInCurParts--;
                            }
                            if (countOfStateInCurParts == 0)
                            {
                                break;
                            }
                        }//берем состояния соотв текущей послед из скобки
                        var chars = GetControlCharsFromCurPart(regExpParts[i]);//управляющие символы из текущей части
                        int stateSelector = 0;
                        foreach (var item in chars)
                        {
                            var curState = tmpStates[stateSelector];
                            stateSelector++;
                            var curSignals = _specialSymbols[$"\\{item}"];
                            foreach (var item1 in curSignals)
                            {
                                int k = StartState._stateID; //ID состояния
                                int l = GetIdByChar(item1, sigma);//ID сигнала
                                result[k, l] = curState._stateID;
                            }
                        }
                    }
                    else
                    {
                        var countOfStateInCurParts = CountNumberOfStates(regExpParts[i]);
                        var tmpStates = new List<State>();
                        for (int j = pointer; j < states.Count; j++)
                        {
                            if (pointer < states.Count)
                            {
                                tmpStates.Add(states[j]);
                                pointer++;
                                countOfStateInCurParts--;
                            }
                            if (countOfStateInCurParts == 0)
                            {
                                break;
                            }
                        }//берем состояния соотв текущей послед из скобки
                        var chars = GetControlCharsFromCurPart(regExpParts[i]);//управляющие символы из текущей части
                        int stateSelector = 0;
                        foreach (var item in chars)
                        {
                            var curState = tmpStates[stateSelector];
                            stateSelector++;
                            var curSignals = _specialSymbols[$"\\{item}"];
                            foreach (var item1 in curSignals)
                            {
                                int k = StartState._stateID; //ID состояния
                                int l = GetIdByChar(item1, sigma);//ID сигнала
                                result[k, l] = curState._stateID;
                                result[curState._stateID, l] = curState._stateID;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public Automaton GetAutomatonByRE(RegularExpression regularExpression)
        {
            string automatonName = regularExpression._regName;
            int automatonPriority = regularExpression._regPriority;
            var states = CreateStatesForAutomaton(SplitByBrackets(regularExpression._regExpression));
            var removeBrekets = string.Concat(SplitByBrackets(regularExpression._regExpression));
            var list = SplitByBrackets(regularExpression._regExpression);
            int n = CountNumberOfStates(removeBrekets) + 1;
            int m = CountNumberOfInputSignals(removeBrekets);
            var sigma = CreateSigma(removeBrekets);
            int[,] delta = GetDelta(n, m, states, sigma, list);
            return new Automaton(automatonName, automatonPriority, states, sigma, delta);
        }
    }
}