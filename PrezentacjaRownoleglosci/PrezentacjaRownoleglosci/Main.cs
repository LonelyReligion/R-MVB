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
            
            List<Producent> producenci = new List<Producent>();
            
            RMVB konsument = new RMVB(kolekcja);

            for (int i = 0; i < 10; i++)
            {
                Symulacja producent = new Symulacja(kolekcja); //moze producentm powinna byc jednosta jakas symulujaca pojedyncze urzadzemnie, mialoby to wiecej sensu
                producenci.Add(producent);
            }

            List<Task> task_producenci = new List<Task>(); //albo cos takiego
            foreach (var producent in producenci)
            {
                producent.Produkuj(); //tu odpalac ale jako taski
            }

            konsument.Konsumuj(); //tez w tle, konczyc tokenem?

        }
    }
}
