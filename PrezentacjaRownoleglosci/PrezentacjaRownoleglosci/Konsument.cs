using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulacja_strumieni
{
    public abstract class Konsument
    {
        BlockingCollection<int> kolekcja = new BlockingCollection<int>();
        public Konsument(BlockingCollection<int> k) {
            kolekcja = k;
        }

        public abstract void Konsumuj();
    }
}
