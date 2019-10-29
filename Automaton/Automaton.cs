using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automaton
{
    internal class Automaton
    {
        public string _automatonName { get; set; }
        public int _priority { get; private set; }
        public List<State> _q { get; private set; }//мн-во состояний
        public Dictionary<int, char> _sigma { get; private set; }// мн-во вх сигналов
        public int[,] _delta { get; private set; }// таблица переходов

        public override string ToString()
        {
            Console.WriteLine("Automaton: {0} priority: {1}", _automatonName, _priority);
            ShowAllStates();
            ShowAllInputSignals();
            ShowDelta();
            return string.Empty;
        }

        public Automaton(string filePath)
        {
            _q = new List<State>();
            using (StreamReader stream = new StreamReader(filePath))
            {
                _priority = int.Parse(stream.ReadLine());
                _automatonName = stream.ReadLine();
                var _states = stream.ReadLine().ToUpper().Trim().Split();
                int _stateID = 0;
                for (int i = 0; i < _states.Length - 1; i++)
                {
                    if (!char.IsDigit(_states[0][0]))
                    {
                        var newState = new State(_stateID, int.Parse(_states[i + 1]), _states[i]);
                        _q.Add(newState);
                        _stateID++;
                        i++;
                    }
                }
                var tmpSigma = stream.ReadLine().Split();
                CreateSigma(tmpSigma);
                int _count = int.Parse(stream.ReadLine());
                int lineCount = _q.Count;
                int columnCount = _sigma.Count;
                _delta = new int[lineCount, columnCount];
                for (int k = 0; k < _count; k++)
                {
                    int i = 0, j = 0;
                    var tmpData = stream.ReadLine().Split();
                    i = GetIdByStateName(tmpData[0]);
                    j = GetIdByChar(char.Parse(tmpData[1]));
                    _delta[i, j] = GetIdByStateName(tmpData[2]);
                }
            }
        }

        private void CreateSigma(string[] tmpSigma)
        {
            _sigma = new Dictionary<int, char>();
            int charID = 0;
            foreach (var item in tmpSigma)
            {
                _sigma.Add(charID, item[0]);
                charID++;
            }
        }

        private void ShowAllStates()
        {
            Console.WriteLine("States: ");
            foreach (var item in _q)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }

        private void ShowAllInputSignals()
        {
            Console.Write("Input signals: ");
            foreach (var item in _sigma)
            {
                Console.Write(item.Value + " ");
            }
            Console.WriteLine();
        }

        private void ShowDelta()
        {
            Console.WriteLine("Delta:");
            for (int i = 0; i < _delta.GetLength(0); i++)
            {
                for (int j = 0; j < _delta.GetLength(1); j++)
                {
                    Console.Write(_delta[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public State GetStartState()
        {
            State result = null;
            foreach (var item in _q)
            {
                if (item._stateType == 0)
                    result = item;
            }
            return result;
        }

        private int GetIdByChar(char c)
        {
            int result = -1;
            foreach (var item in _sigma)
            {
                if (c == item.Value)
                {
                    result = item.Key;
                }
            }
            return result;
        }

        private int GetIdByStateName(string name)
        {
            int result = 0;
            foreach (var item in _q)
            {
                if (item._stateName == name)
                {
                    result = item._stateID;
                }
            }
            return result;
        }

        public int GetIdFromSigmaByChar(char c)
        {
            return GetIdByChar(c);
        }

        public int[] GetLineFromMatrixByState(State state)
        {
            int[] result = new int[_delta.GetLength(1)];
            for (int k = 0; k < result.Length; k++)
            {
                result[k] = _delta[state._stateID, k];
            }
            return result;
        }

        public State GetStateByID(int id)
        {
            State result = null;
            foreach (var item in _q)
            {
                if (item._stateID == id)
                {
                    result = item;
                }
            }
            return result;
        }

        public KeyValuePair<bool, int> MaxStr(string str, int position)
        {
            bool flag = false;
            int maxLength = 0;
            State curState = GetStartState();
            int[,] delta = _delta;
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
                int[] curStateValues = GetLineFromMatrixByState(curState);
                int index = GetIdFromSigmaByChar(str[i]);
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
                    curState = GetStateByID(delta[curState._stateID, index]);
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

        public bool isSymbolInSigma(char c)
        {
            return _sigma.Values.Contains(c);
        }
    }
}