namespace Automaton
{
    internal class RegularExpression
    {
        public string _regName { get; private set; }
        public int _regPriority { get; private set; }
        public string _regExpression { get; private set; }

        public RegularExpression(string regClass, int priority, string reg)
        {
            _regName = regClass;
            _regPriority = priority;
            _regExpression = reg;
        }

        public override string ToString()
        {
            return $"{_regName} {_regPriority} {_regExpression}";
        }
    }
}