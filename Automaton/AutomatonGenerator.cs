using System.Collections.Generic;
using System.IO;

namespace Automaton
{
    internal class AutomatonGenerator
    {
        public List<RegularExpression> _regStorage { get; private set; }

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
            }
        }
    }
}