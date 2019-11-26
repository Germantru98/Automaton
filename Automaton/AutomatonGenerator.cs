using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automaton
{
    internal class AutomatonGenerator
    {
        public List<RegularExpression> _regStorage { get; private set; }
        public Dictionary<string, List<string>> _specialSymbols { get; private set; }

        private int _stateCounter = 0;

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

        public AutomatonGenerator()
        {
            setSpecialSymbols();
        }

        private List<string> getChars(string fileName, int modifier)//0 - без модификатора, 1 - Upper, 2 - Lower
        {
            using (StreamReader stream = new StreamReader(fileName))
            {
                List<string> result = new List<string>();
                if (modifier == 0)
                {
                    var str = stream.ReadToEnd();
                    foreach (var item in str)
                    {
                        result.Add(item.ToString());
                    }
                }
                else if (modifier == 1)
                {
                    var str = stream.ReadToEnd().ToUpper();
                    foreach (var item in str)
                    {
                        result.Add(item.ToString());
                    }
                }
                else
                {
                    var str = stream.ReadToEnd().ToLower();
                    foreach (var item in str)
                    {
                        result.Add(item.ToString());
                    }
                }
                return result;
            }
        }

        private List<string> getWS()
        {
            string[] chars = { "\n", " ", "\t", "\r" };
            return chars.ToList();
        }

        private void setSpecialSymbols()
        {
            _specialSymbols = new Dictionary<string, List<string>>();
            _specialSymbols.Add("\\W", getChars("Alphabet.txt", 1));
            _specialSymbols.Add("\\w", getChars("Alphabet.txt", 2));
            _specialSymbols.Add("\\d", getChars("Digits.txt", 0));
            _specialSymbols.Add("\\s", getWS());
            _specialSymbols.Add("\\_", getChars("_.txt", 0));
            _specialSymbols.Add("\\-", getChars("-.txt", 0));
            _specialSymbols.Add("\\+", getChars("+.txt", 0));
            _specialSymbols.Add("\\{", getChars("{.txt", 0));
            _specialSymbols.Add("\\}", getChars("}.txt", 0));
        }

        private List<string> SplitByBrackets(string re)
        {
            List<string> result = new List<string>();
            int i = 0;
            while (i < re.Length)
            {
                var tmpStr = string.Empty;
                if (re[i] == '(')
                {
                    i++;
                    while (re[i] != ')')
                    {
                        tmpStr += re[i];
                        i++;
                    }
                }
                else
                {
                    tmpStr += re[i];
                }
                i++;
                if (!string.IsNullOrEmpty(tmpStr))
                {
                    result.Add(tmpStr);
                }
            }
            return result;
        }

        ///<summary>Метод разбивает регулярное выражение на отдельные блоки</summary>
        ///<param name="RPN">Регулярное выражение в форме ОПЗ</param>
        public List<string> SplitRPN(string RPN)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < RPN.Length; i++)
            {
                if (RPN[i] == '\\')
                {
                    var tmpItem = RPN[i].ToString() + RPN[i + 1].ToString();
                    i++;
                    result.Add(tmpItem);
                }
                else
                {
                    result.Add(RPN[i].ToString());
                }
            }
            return result;
        }

        private void ActionWithAutomatons(Automaton op1, Automaton op2, string action)
        {
            switch (action)
            {
                case "|":
                    VerticalBarAction(op1, op2);
                    break;

                case "·":
                    ConcatAutomatonsAction(op1, op2);
                    break;

                default:
                    break;
            }
        }

        public void VerticalBarAction(Automaton op1, Automaton op2)
        {
            op1._automatonName += $"|{op2._automatonName}";
            AddSigmaToSigma(op1._sigma, op2._sigma);
            var op2StartState = op2.GetStartState();
            var op1StartState = op1.GetStartState();
            foreach (var line in op2._delta[op2StartState])
            {
                op1._delta[op1StartState].Add(new Line(op1StartState, line._symbol, line._to));
            }
            foreach (var item in op2._delta)
            {
                if (item.Key._stateType != 0)
                {
                    op1._delta.Add(item.Key, item.Value);
                }
            }
        }

        public void ConcatAutomatonsAction(Automaton op1, Automaton op2)
        {
            op1._automatonName += $"·{op2._automatonName}";
            AddSigmaToSigma(op1._sigma, op2._sigma);
            var op2StartState = op2.GetStartState();
            foreach (var state in op1._delta.Keys)
            {
                if (state._stateType == 2)
                {
                    state._stateType = 1;
                    foreach (var line in op2._delta[op2StartState])
                    {
                        op1._delta[state].Add(new Line(state, line._symbol, line._to));
                    }
                }
            }
            foreach (var item in op2._delta)
            {
                if (item.Key._stateType != 0)
                {
                    op1._delta.Add(item.Key, item.Value);
                }
            }
        }

        public void Iterration(Automaton op1)
        {
            var pairs = new Dictionary<State, List<string>>();
            foreach (var state in op1._delta.Keys)
            {
                foreach (var line in op1._delta[state])
                {
                    if (line._to._stateType == 2)
                    {
                        var symbol = line._symbol;
                        var symbols = new List<string>();
                        if (!pairs.ContainsKey(line._to))
                        {
                            symbols.Add(symbol);
                            pairs.Add(line._to, symbols);
                        }
                        else
                        {
                            pairs[line._to].Add(symbol);
                        }
                    }
                }
            }
            foreach (var startState in pairs.Keys)
            {
                foreach (var finishState in pairs.Keys)
                {
                    foreach (var symbol in pairs[finishState])
                    {
                        op1._delta[startState].Add(new Line(startState, symbol, finishState));
                    }
                }
            }
            op1._automatonName += "*";
        }

        public Automaton GetAutomatonBySpecialSymbol(string specialSymbol)
        {
            var sigma = _specialSymbols[specialSymbol];
            string name = specialSymbol;
            int priority = 0;
            var delta = CreateDelta(sigma);
            Automaton newAutomaton = new Automaton(name, priority, sigma, delta);
            return newAutomaton;
        }

        public Automaton GetAutomatonBySymbol(string symbol)
        {
            List<string> sigma = new List<string>();
            sigma.Add(symbol);
            string name = symbol;
            int priority = 0;
            var delta = CreateDelta(symbol);
            Automaton newAutomaton = new Automaton(name, priority, sigma, delta);
            return newAutomaton;
        }

        private Dictionary<State, List<Line>> CreateDelta(List<string> symbols)
        {
            int id = 0;
            State startState = new State(id, 0, $"S{_stateCounter}");
            id++;
            _stateCounter++;
            State finishState = new State(id, 2, $"S{_stateCounter}");
            _stateCounter++;
            List<Line> lines = new List<Line>();
            foreach (var symbol in symbols)
            {
                lines.Add(new Line(startState, symbol, finishState));
            }
            Dictionary<State, List<Line>> newDelta = new Dictionary<State, List<Line>>();
            newDelta.Add(startState, lines);
            newDelta.Add(finishState, new List<Line>());
            return newDelta;
        }

        private Dictionary<State, List<Line>> CreateDelta(string symbol)
        {
            int id = 0;
            State startState = new State(id, 0, $"S{_stateCounter}");
            id++;
            _stateCounter++;
            State finishState = new State(id, 2, $"S{_stateCounter}");
            _stateCounter++;
            Line line = new Line(startState, symbol, finishState);
            List<Line> lines = new List<Line>();
            lines.Add(line);
            Dictionary<State, List<Line>> newDelta = new Dictionary<State, List<Line>>();
            newDelta.Add(startState, lines);
            newDelta.Add(finishState, new List<Line>());
            return newDelta;
        }

        private void AddListToList(List<Line> op1, List<Line> op2)
        {
            foreach (var item in op2)
            {
                op1.Add(item);
            }
        }

        private void AddSigmaToSigma(List<string> op1, List<string> op2)
        {
            foreach (var item in op2)
            {
                if (!op1.Contains(item))
                {
                    op1.Add(item);
                }
            }
        }

        public Automaton CreateAutomatonByRE(RegularExpression RE)
        {
            var RPN = ReversePN.ToRPN(RE._regExpression);
            System.Console.WriteLine(RPN);
            var RPNParts = SplitRPN(RPN);
            Stack<Automaton> stack = new Stack<Automaton>();
            foreach (var elem in RPNParts)
            {
                if (isOperation(elem))
                {
                    if (elem != "*" && stack.Count > 1)
                    {
                        var op2 = stack.Pop();
                        var op1 = stack.Pop();
                        ActionWithAutomatons(op1, op2, elem);
                        stack.Push(op1);
                    }
                    else
                    {
                        var op1 = stack.Pop();
                        Iterration(op1);
                        stack.Push(op1);
                    }
                }
                else
                {
                    if (isSpecialSymbol(elem))
                    {
                        stack.Push(GetAutomatonBySpecialSymbol(elem));
                    }
                    else
                    {
                        stack.Push(GetAutomatonBySymbol(elem));
                    }
                }
            }
            Automaton result = stack.Pop();
            result._automatonName = RE._regName;
            result._priority = RE._regPriority;
            return result;
        }

        private bool isOperation(string elem)
        {
            if (elem == "|")
            {
                return true;
            }
            else if (elem == "*")
            {
                return true;
            }
            else if (elem == "·")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isSpecialSymbol(string elem)
        {
            if (elem.Contains("\\"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Automaton> GetAutomatonsByRE()
        {
            List<Automaton> automatons = new List<Automaton>();
            foreach (var re in _regStorage)
            {
                automatons.Add(CreateAutomatonByRE(re));
            }
            return automatons;
        }

    }
}