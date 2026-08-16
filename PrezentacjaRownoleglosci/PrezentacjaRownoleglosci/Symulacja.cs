using Symulacja_strumieni;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacjaRownoleglosci
{
    internal class Symulacja : Producent
    {
        public Symulacja(BlockingCollection<int> kolekcja) : base(kolekcja)
        { 
        
        }

        public override void Produkuj()
        {
            throw new NotImplementedException();
        }
    }
}
