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
            CancellationTokenSource zrodlo = new CancellationTokenSource();
            CancellationToken token = zrodlo.Token;

            BlockingCollection<object> kolekcja = new BlockingCollection<object>();
            
            RMVB konsument = new RMVB(kolekcja, (10 + (10*10)));

            Symulacja producent = new Symulacja(kolekcja, 10); //moze producentm powinna byc jednosta jakas symulujaca pojedyncze urzadzemnie, mialoby to wiecej sensu
            producent.zdefiniujLiczbePomiarow(10);

            Thread watek_konsumenta = new Thread(konsument.Konsumuj);
            producent.Produkuj();
            watek_konsumenta.Start();

            producent.ZakonczProdukcje();
            watek_konsumenta.Join();
            
        }
    }
}
