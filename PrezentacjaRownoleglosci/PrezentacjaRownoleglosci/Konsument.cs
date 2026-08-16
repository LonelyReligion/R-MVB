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
        protected BlockingCollection<object> kolekcja = new BlockingCollection<object>();
        public Konsument(BlockingCollection<object> k) {
            kolekcja = k;
        }

        public abstract void Konsumuj();
    }
}
