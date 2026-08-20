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
            int liczba_urzadzen = 10;
            int liczba_pomiarow = 10;

            BlockingCollection<object> kolekcja = new BlockingCollection<object>();
            
            RMVB konsument = new RMVB(kolekcja);

            Symulacja producent = new Symulacja(kolekcja, liczba_urzadzen); //moze producentm powinna byc jednosta jakas symulujaca pojedyncze urzadzemnie, mialoby to wiecej sensu
            producent.zdefiniujLiczbePomiarow(liczba_pomiarow);

            Thread watek_konsumenta = new Thread(()=>konsument.Konsumuj());
            watek_konsumenta.Start();
            producent.Produkuj();
            
            producent.ZakonczProdukcje();

            watek_konsumenta.Join();
        }
    }
}
