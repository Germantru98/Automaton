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

        private State GetStateByID(List<State> states, int id)
        {
            State result = new State(0, 0, "S0");
            foreach (var item in states)
            {
                if (item._stateID == id)
                {
                    result = item;
                }
            }
            return result;
        }

        public Automaton GetAutomaton(RegularExpression regularExpression)
        {
            var splitByBreckets = SplitByBrackets(regularExpression._regExpression);
            List<int> iterationsPos = SearchStarInStr(splitByBreckets);
            var sigma = CreateSigma(string.Concat(splitByBreckets));
            int n = CountNumberOfStates(string.Concat(splitByBreckets)) + 1;
            int m = CountNumberOfInputSignals(string.Concat(splitByBreckets));
            int[,] delta = new int[n, m];
            var states = CreateStatesForAutomaton(splitByBreckets);
            Queue<State> tempStates = new Queue<State>();
            tempStates.Enqueue(GetStateByID(states, 0));//добавляем в стартовые состояние состояние S0;
            for (int i = 0; i < splitByBreckets.Count; i++)//цикл проходит по содержимому скобок
            {
                var tmpStartPos = tempStates.Dequeue()._stateID;
                if (splitByBreckets[i] != "*")//проверяем не явл ли текущая часть *
                {
                    int curStartState = tmpStartPos;
                    int curFinsihState = curStartState + 1;
                    if (!iterationsPos.Contains(i + 1))//проверяем не явл след часть *
                    {
                        var splitByVB = SplitByVerticalBar(splitByBreckets[i]);//делим содержимое текущей скобки на части между  |
                        for (int j = 0; j < splitByVB.Count; j++)//цикл проходит по частям между | и делит их на управляющие символы
                        {
                            var controlSybols = GetControlCharsFromCurPart(splitByVB[j]);//получены символы для текущей части до |
                            int count = controlSybols.Count;
                            for (int k = 0; k < count; k++)//цикл проходит по текущим управляющим символам и добавляет в матрицу переходов соотв данные
                            {
                                var curSymbol = controlSybols[k];

                                var curSignals = _specialSymbols[$"\\{curSymbol}"];
                                foreach (var item in curSignals)
                                {
                                    int curCharID = GetIdByChar(item, sigma);
                                    delta[curStartState, curCharID] = curFinsihState;
                                }
                                var tmpState = GetStateByID(states, curFinsihState);
                                if (tmpState._stateType == 2)//проверка на конечное состояние
                                {
                                    tempStates.Enqueue(tmpState);
                                }
                                else
                                {
                                    curStartState++;
                                    curFinsihState++;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Переход в текущую область происходит если после скобки есть звездочка (*) Данный кусок практически не отличается
                        //от части где нет *, за исключением одного добавленного куска кода в конце.
                        Dictionary<string, int> pairs = new Dictionary<string, int>();
                        var splitByVB = SplitByVerticalBar(splitByBreckets[i]);//делим содержимое текущей скобки на части между  |
                        for (int j = 0; j < splitByVB.Count; j++)//цикл проходит по частям между | и делит их на управляющие символы
                        {
                            //
                            var controlSybols = GetControlCharsFromCurPart(splitByVB[j]);//получены символы для текущей части до |
                            int count = controlSybols.Count;
                            for (int k = 0; k < count; k++)//цикл проходит по текущим управляющим символам и добавляет в матрицу переходов соотв данные
                            {
                                var curSymbol = controlSybols[k];

                                var curSignals = _specialSymbols[$"\\{curSymbol}"];
                                foreach (var item in curSignals)
                                {
                                    int curCharID = GetIdByChar(item, sigma);
                                    delta[curStartState, curCharID] = curFinsihState;
                                }
                                pairs.Add(curSymbol.ToString(), curFinsihState);//заведен словарь где хранится упр символ - айди состояния к которому привязан данный символ
                                var tmpState = GetStateByID(states, curFinsihState);
                                if (tmpState._stateType == 2)//проверка на конечное состояние
                                {
                                    tempStates.Enqueue(tmpState);
                                }
                                else
                                {
                                    curStartState++;
                                    curFinsihState++;
                                }
                            }
                            foreach (var x in pairs)// в двойном цикле проходим по элементам словаря и добавляем переходы между состояними по их управляющим символам.
                            {                       // проще говоря соединяем между собой состояния соотв сигналами , а также добавляем "петли".
                                foreach (var z in pairs)
                                {
                                    var curSignals = _specialSymbols[$"\\{z.Key}"];
                                    foreach (var item in curSignals)
                                    {
                                        int curCharID = GetIdByChar(item, sigma);
                                        delta[x.Value, curCharID] = z.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var result = new Automaton(regularExpression._regName, regularExpression._regPriority, states, sigma, delta);
            return result;
        }
    }
}