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
        internal RTreeAdapter(RTree drzewo)
        {
            this.drzewo = drzewo;
        }

        //wypisz
        //dodaj
        internal void dodajUrzadzenie(Urzadzenie u) {
            drzewo.Insert(u);
        }

        internal void dodajPomiar(int id, Pomiar p) {
            drzewo.AddMeasure(id, p);
        }

        internal void dodajPomiar(int ix, DateTime t, Decimal v) {
            drzewo.AddMeasure(ix, t, v);
        }

        //usun -- nie istnieje
        internal void usunUrzadzenie(Urzadzenie u) { 
            throw new NotImplementedException();
        }

        //szukaj
        //zwraca listę urządzeń znajdujących się w zadanym prostokącie
        internal List<Urzadzenie> szukaj(Rectangle rect) {
            return drzewo.SearchBy(rect);
        }

        //zwraca urządzenie w podanym punkcie
        internal Urzadzenie szukaj(decimal x, decimal y) { 
            return drzewo.SearchBy(x, y);
        }

        //zwraca liczbę pomiarów i agregat powierzchniowy (z czego?)
        internal (decimal, decimal) szukajAgregatuPowierzchniowego(Rectangle rect) { 
            return drzewo.FindSpaceAggregate(rect);
        }

        //zwraca agregat czasowy
        public decimal szukajAgregatuCzasowego(decimal x, decimal y) {
            return drzewo.GetTimeAggregate(x, y);
        }

        public void obliczAgregaty() {
            drzewo.SpaceAggregate();
        }
    }
}
