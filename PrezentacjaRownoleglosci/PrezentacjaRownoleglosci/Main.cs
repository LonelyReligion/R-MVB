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
            BlockingCollection<object> kolekcja = new BlockingCollection<object>();
            
            RMVB konsument = new RMVB(kolekcja);

            Symulacja producent = new Symulacja(kolekcja); //moze producentm powinna byc jednosta jakas symulujaca pojedyncze urzadzemnie, mialoby to wiecej sensu
            producent.zdefiniujLiczbeUrzadzen(10);
            producent.zdefiniujLiczbePomiarow(10);
            producent.Produkuj();

            konsument.Konsumuj(); //tez w tle, konczyc tokenem?

        }
    }
}
