namespace Automaton
{
    internal class State
    {
        public int _stateID { get; set; }
        public int _stateType { get; set; }//начальное-0 или конечноt-1 или 2
        public string _stateName { get; set; }

        public State(int id, int statusPos, string name)
        {
            _stateID = id;
            _stateType = statusPos;
            _stateName = name;
        }

        public override string ToString()
        {
            return $"{_stateID}. {_stateName} type: {_stateType}";
        }
    }
}