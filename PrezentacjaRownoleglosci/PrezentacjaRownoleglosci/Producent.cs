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
        protected BlockingCollection<object> kolekcja;
        static int najwyzsze_id = 1;
        
        public Producent(BlockingCollection<object> kolekcja)
        {
            this.kolekcja = kolekcja;
        }

        public abstract void Produkuj();

    }
}
