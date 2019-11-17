using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automaton
{
    internal class AutomatonGenerator
    {
        public List<RegularExpression> _regStorage { get; private set; }
        public Dictionary<string, List<char>> _specialSymbols { get; private set; }

        private MatrixHandler matrixHandler = new MatrixHandler();

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
            int stateCounter = 1;
            var splitByBreckets = SplitByBrackets(regularExpression._regExpression);
            List<int> iterationsPos = SearchStarInStr(splitByBreckets);
            var sigma = CreateSigma(string.Concat(splitByBreckets));
            int n = 1;
            int m = CountNumberOfInputSignals(string.Concat(splitByBreckets));
            int[,] delta = new int[n, m];
            var states = new List<State>();
            states.Add(new State(0, 0, "S0"));
            Queue<State> tempStates = new Queue<State>();
            tempStates.Enqueue(GetStateByID(states, 0));//добавляем в стартовые состояние состояние S0;
            int curStartState;
            var curStateStorage = new List<State>();//для каждой итерации храним в очереди все конечные состояние из прошлой
            for (int i = 0; i < splitByBreckets.Count; i++)//цикл проходит по содержимому скобок
            {
                curStateStorage.Clear();
                while (tempStates.Count > 0)
                {
                    var ts = tempStates.Dequeue();
                    curStateStorage.Add(ts);
                    System.Console.WriteLine("Достали из очереди в лист {0}", ts._stateID);
                }
                foreach (var state in curStateStorage)
                {
                    if (splitByBreckets[i] != "*")//проверяем не явл ли текущая часть *
                    {
                        curStartState = state._stateID;
                        int curFinsihState;
                        if (!iterationsPos.Contains(i + 1))//проверяем не явл след часть *
                        {
                            var splitByVB = SplitByVerticalBar(splitByBreckets[i]);
                            for (int j = 0; j < splitByVB.Count; j++)//цикл проходит по частям между | и делит их на управляющие символы
                            {
                                curStartState = state._stateID;
                                System.Console.WriteLine("Для {0} CSS = {1}", splitByVB[j], curStartState);
                                var controlSymbols = GetControlCharsFromCurPart(splitByVB[j]);//получены символы для текущей части до |
                                int count = controlSymbols.Count;
                                for (int k = 0; k < count; k++)//цикл проходит по текущим управляющим символам и добавляет в матрицу переходов соотв данные
                                {
                                    var curSymbol = controlSymbols[k];
                                    if (i == splitByBreckets.Count - 1)
                                    {
                                        if (j < count - 1)
                                        {
                                            var newState = new State(stateCounter, 1, $"S{stateCounter}");
                                            states.Add(newState);
                                            stateCounter++;
                                            curFinsihState = newState._stateID;
                                            delta = matrixHandler.AddLine(new int[m], delta);
                                        }
                                        else
                                        {
                                            var newState = new State(stateCounter, 2, $"S{stateCounter}");
                                            states.Add(newState);
                                            stateCounter++;
                                            curFinsihState = newState._stateID;
                                            delta = matrixHandler.AddLine(new int[m], delta);
                                        }
                                    }
                                    else
                                    {
                                        var newState = new State(stateCounter, 1, $"S{stateCounter}");
                                        states.Add(newState);
                                        stateCounter++;
                                        curFinsihState = newState._stateID;
                                        delta = matrixHandler.AddLine(new int[m], delta);
                                    }
                                    var curSignals = _specialSymbols[$"\\{curSymbol}"];
                                    foreach (var item in curSignals)
                                    {
                                        int curCharID = GetIdByChar(item, sigma);
                                        delta[curStartState, curCharID] = curFinsihState;
                                    }
                                    var tmpState = GetStateByID(states, curFinsihState);
                                    tempStates.Enqueue(tmpState);
                                    System.Console.WriteLine("помещаем {0}", tmpState);
                                    System.Console.WriteLine("для {0} CFS = {1}", curSymbol, curFinsihState);
                                }
                            }
                        }
                        else
                        {
                            Dictionary<string, int> pairs = new Dictionary<string, int>();
                            var splitByVB = SplitByVerticalBar(splitByBreckets[i]);
                            for (int j = 0; j < splitByVB.Count; j++)//цикл проходит по частям между | и делит их на управляющие символы
                            {
                                curStartState = state._stateID;
                                System.Console.WriteLine("Для {0} CSS = {1}", splitByVB[j], curStartState);
                                var controlSymbols = GetControlCharsFromCurPart(splitByVB[j]);//получены символы для текущей части до |
                                int count = controlSymbols.Count;
                                for (int k = 0; k < count; k++)//цикл проходит по текущим управляющим символам и добавляет в матрицу переходов соотв данные
                                {
                                    var curSymbol = controlSymbols[k];
                                    if (i == splitByBreckets.Count - 2)
                                    {
                                        if (j < count - 1)
                                        {
                                            var newState = new State(stateCounter, 1, $"S{stateCounter}");
                                            states.Add(newState);
                                            stateCounter++;
                                            curFinsihState = newState._stateID;
                                            delta = matrixHandler.AddLine(new int[m], delta);
                                        }
                                        else
                                        {
                                            var newState = new State(stateCounter, 2, $"S{stateCounter}");
                                            states.Add(newState);
                                            stateCounter++;
                                            curFinsihState = newState._stateID;
                                            delta = matrixHandler.AddLine(new int[m], delta);
                                        }
                                    }
                                    else
                                    {
                                        var newState = new State(stateCounter, 1, $"S{stateCounter}");
                                        states.Add(newState);
                                        stateCounter++;
                                        curFinsihState = newState._stateID;
                                        delta = matrixHandler.AddLine(new int[m], delta);
                                    }
                                    var curSignals = _specialSymbols[$"\\{curSymbol}"];
                                    foreach (var item in curSignals)
                                    {
                                        int curCharID = GetIdByChar(item, sigma);
                                        delta[curStartState, curCharID] = curFinsihState;
                                    }
                                    pairs.Add(curSymbol.ToString(), curFinsihState);
                                    var tmpState = GetStateByID(states, curFinsihState);
                                    tempStates.Enqueue(tmpState);
                                    System.Console.WriteLine("помещаем {0}", tmpState);
                                    System.Console.WriteLine("для {0} CFS = {1}", curSymbol, curFinsihState);
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