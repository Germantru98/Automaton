namespace Automaton
{
    internal class RegularExpression
    {
        public string _regClass { get; private set; }
        public int _regPriority { get; private set; }
        public string _regExpression { get; private set; }

        public RegularExpression(string regClass, int priority, string reg)
        {
            _regClass = regClass;
            _regPriority = priority;
            _regExpression = reg;
        }

        public override string ToString()
        {
            return $"{_regClass} {_regPriority} {_regExpression}";
        }
    }
}