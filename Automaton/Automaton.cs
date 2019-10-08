using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    class Automaton
    {
        public List<State> Q { get; set; }//мн-во состояний
        public int[] Sigma { get; set; }// мн-во вх сигналов
        public int[,] Delta { get; set; }// таблица переходов
        public List<string> S { get; set; }//нач сост
        public List<string> F { get; set; }//конечные сост
        public override string ToString()
        {
            return base.ToString();
        }
        public Automaton(string filePath)
        {
            Q = new List<State>();
            using (StreamReader stream = new StreamReader(filePath))
            {
                var _states = stream.ReadLine().ToUpper().Trim().Split();
                foreach (var item in _states)
                {
                    Console.WriteLine(item);
                }
                int _stateID = 0;
                for (int i = 0; i < _states.Length - 1; i++)
                {
                    if (!char.IsDigit(_states[0][0]))
                    {
                        var newState = new State(_stateID, int.Parse(_states[i + 1]), _states[i]);
                        Q.Add(newState);
                        _stateID++;
                        i++;
                    }
                }
            }
        }
        public void showAllStates()
        {
            foreach (var item in Q)
            {
                Console.WriteLine(item);
            }
        }
    }
}
