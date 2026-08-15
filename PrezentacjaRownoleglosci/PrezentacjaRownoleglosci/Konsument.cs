using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulacja_strumieni
{
    public class Konsument
    {
        BlockingCollection<int> kolekcja = new BlockingCollection<int>();
        public Konsument(BlockingCollection<int> k) {
            kolekcja = k;
        }

        internal void Konsumuj()
        {
            Console.WriteLine("Tu konsument. Przyjmuje dane o numerze " + kolekcja.Take() + ".");
        }
    }
}
