using Symulacja_strumieni;
using Symulacja_strumieni.model;
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
        public Symulacja(BlockingCollection<object> kolekcja) : base(kolekcja)
        { 
        
        }

        public override void Produkuj()
        {
            Urzadzenie testowe = new Urzadzenie((15.0m, 53.0m));
            Console.WriteLine("Wysylam przykladowe urządzenie.");
            base.kolekcja.Add(testowe);
        }

    }
}
