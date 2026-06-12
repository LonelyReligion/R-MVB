using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulacja_strumieni
{
    public class Producent
    {
        BlockingCollection<int> kolekcja;
        private int id;
        static int najwyzsze_id = 1;
        
        public Producent(BlockingCollection<int> kolekcja)
        {
            this.kolekcja = kolekcja;
            id = najwyzsze_id++;
        }

        public void Produkuj() {
            Console.WriteLine("Z tej strony producent nr. " + id.ToString() + ". Produkuję.");
            kolekcja.Add(this.id);
        }
    }
}
