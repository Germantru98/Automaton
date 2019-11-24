namespace Automaton
{
    public class Line
    {
        public State _from { get; set; }
        public State _to { get; set; }
        public string _symbol { get; set; }

        public override string ToString()
        {
            return $"{_from._stateName} -> {_to._stateName} by {_symbol}";
        }

        public Line(State from, string symbol, State to)
        {
            _from = from;
            _symbol = symbol;
            _to = to;
        }
    }
}