using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using PrezentacjaRownoleglosci;
using Symulacja_strumieni.rmvb;

namespace Symulacja_strumieni
{
    public class main
    {
        public static void Main()
        {
            BlockingCollection<int> kolekcja = new BlockingCollection<int>();
            
            List<Producent> producenci = new List<Producent>();
            
            RMVB konsument = new RMVB(kolekcja);
            Symulacja producent =  new Symulacja(kolekcja);

            
            producent.Produkuj();

            konsument.Konsumuj();
        }
    }
}
