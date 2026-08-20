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
        int _liczbaPomiarow;
        List<Task> ts = new List<Task>();
        int _liczbaurzadzen;
        public Symulacja(BlockingCollection<object> kolekcja, int liczbaUrzadzen) : base(kolekcja)
        {
            _liczbaurzadzen = liczbaUrzadzen;
        }

        public override void Produkuj()
        {
            for (int i = 0; i < _liczbaurzadzen; i++)
            {
                ts.Add(Task.Run(zadanie));
            }
        }

        public override void ZakonczProdukcje()
        {
            Task.WaitAll(ts.ToArray());
            kolekcja.CompleteAdding();
        }

        private async Task zadanie() {
            Urzadzenie nowe = new Urzadzenie(generujWspolrzedne());
            Console.WriteLine("Tu urzadzenie o id: " + nowe.UrzadzenieID + ". Zaczynam pracę.");
            base.kolekcja.Add(nowe);

            for (int i = 0; i < _liczbaPomiarow; i++)
            {
                (int, Pomiar) nowy = (nowe.UrzadzenieID, generujLosowyPomiar());
                //przy tworzeniu wersji rozmawiamy zarowno z repo jak i z rmvb, to musi robic juz konsument
                base.kolekcja.Add(nowy);
                await Task.Delay(500); // pol selundy
                
            }

            Console.WriteLine("Tu urzadzenie o id: " + nowe.UrzadzenieID + ". Kończę pracę.");

        }

        internal void zdefiniujLiczbePomiarow(int v) {
            _liczbaPomiarow = v;
        }

        public (Decimal, Decimal) generujWspolrzedne()
        {
            Decimal szerokosc = (Decimal)(Random.Shared.Next(49, 55) * 100);
            if (szerokosc < 5400)
            {
                szerokosc += Random.Shared.Next(00, 59);
            }
            else
            {
                szerokosc += Random.Shared.Next(00, 50);
            }
            szerokosc = szerokosc / 100.0m;

            Decimal dlugosc = (Decimal)(Random.Shared.Next(14, 24) * 100);
            if (dlugosc < 2400)
            {
                dlugosc += Random.Shared.Next(07, 59);
            }
            else
            {
                dlugosc += Random.Shared.Next(00, 09);
            }
            dlugosc = dlugosc / 100.0m;

            return (dlugosc, szerokosc);
        }

        public Pomiar generujLosowyPomiar()
        {
            Decimal temp = (Decimal)(Random.Shared.NextDouble() * (41.0 - (-41.0)) - 41.0);
            Pomiar testowy = new Pomiar(temp, DateTime.Now);
            return testowy;
        }

    }
}
