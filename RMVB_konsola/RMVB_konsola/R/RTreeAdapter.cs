using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.R
{
    internal class RTreeAdapter
    {
        RTree drzewo;
        public RTreeAdapter(RTree drzewo)
        {
            this.drzewo = drzewo;
        }

        //wypisz
        //dodaj
        public void dodajUrzadzenie(Urzadzenie u) {
            drzewo.Insert(u);
        }

        public void dodajPomiar(int id, Pomiar p) {
            drzewo.AddMeasure(id, p);
        }

        public void dodajPomiar(int ix, DateTime t, Decimal v) {
            drzewo.AddMeasure(ix, t, v);
        }

        //usun -- nie istnieje
        public void usunUrzadzenie(Urzadzenie u) { 
            throw new NotImplementedException();
        }

        //szukaj
        //zwraca listę urządzeń znajdujących się w zadanym prostokącie
        public List<Urzadzenie> szukaj(Rectangle rect) {
            return drzewo.SearchBy(rect);
        }

        //zwraca urządzenie w podanym punkcie
        public Urzadzenie szukaj(decimal x, decimal y) { 
            return drzewo.SearchBy(x, y);
        }

        //zwraca liczbę pomiarów i agregat czasowy (z czego?)
        public (decimal, decimal) szukajAgregatu(Rectangle rect) { 
            return drzewo.FindSpaceAggregate(rect);
        }

    }
}
