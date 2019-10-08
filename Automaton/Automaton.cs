using System;
using System.Collections.Generic;
using System.IO;

namespace Automaton
{
    internal class Automaton
    {
        public List<State> _q { get; private set; }//мн-во состояний
        public Dictionary<int, char> _sigma { get; private set; }// мн-во вх сигналов
        public int[,] _delta { get; private set; }// таблица переходов

        public override string ToString()
        {
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

                //переделать составления матрицы, где i это состояния, а j это сигналы.
                int lineCount = _q.Count;
                int columnCount = _sigma.Count;
                _delta = new int[lineCount, columnCount];
                for (int k = 0; k < _count; k++)
                {
                    int i = 0, j = 0;
                    var tmpData = stream.ReadLine().Split(); ;
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
                if (item._statePosition == 0)
                    result = item;
            }
            return result;
        }

        private int GetIdByChar(char c)
        {
            int result = 888;
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
    }
}