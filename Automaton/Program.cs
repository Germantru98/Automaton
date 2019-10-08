using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton
{
    class Program
    {
        static void Main(string[] args)
        {
            Automaton a = new Automaton("Automaton_1.txt");
            a.showAllStates();
        }
    }
}
