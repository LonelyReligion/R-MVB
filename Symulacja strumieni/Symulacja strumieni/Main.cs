using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Symulacja_strumieni
{
    public class main
    {
        public static void Main()
        {
            BlockingCollection<int> kolekcja = new BlockingCollection<int>();

            List<Producent> producenci = new List<Producent>();
            
            Konsument konsument = new Konsument(kolekcja);
            Producent producent = new Producent(kolekcja);
            
            producent.Produkuj();
            konsument.Konsumuj();
        }
    }
}
