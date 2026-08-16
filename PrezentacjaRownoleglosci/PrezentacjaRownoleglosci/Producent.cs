using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulacja_strumieni
{
    public abstract class Producent
    {
        BlockingCollection<int> kolekcja;
        static int najwyzsze_id = 1;
        
        public Producent(BlockingCollection<int> kolekcja)
        {
            this.kolekcja = kolekcja;
        }

        public abstract void Produkuj();

    }
}
