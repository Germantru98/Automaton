using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    class State
    {
        public int __stateID { get; set; }
        public int _statePosition { get; set; }//начальное-0 или конечноt-1 или 2
        public string _stateName { get; set; }
        public State(int id, int statusPos,string name)
        {
            __stateID = id;
            _statePosition = statusPos;
            _stateName = name;
        }
        public override string ToString()
        {
            return $"{__stateID}. {_stateName} {_statePosition}";
        }
    }
}
