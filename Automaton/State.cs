namespace Automaton
{
    public class State
    {
        public int _stateType { get; set; }//начальное - 0 или конечное - 1 или 2
        public string _stateName { get; set; }

        public State(int id, int statusPos, string name)
        {
            _stateType = statusPos;
            _stateName = name;
        }

        public override string ToString()
        {
            return $"{_stateName} type: {_stateType}";
        }

        public override int GetHashCode()
        {
            return _stateName.GetHashCode();
        }
    }
}