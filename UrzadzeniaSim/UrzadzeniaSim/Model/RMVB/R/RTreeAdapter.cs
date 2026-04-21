namespace UrzadzeniaSim.Model.RMVB.R
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
        internal void dodajUrzadzenie(Urzadzenie_Model u) {
            drzewo.Insert(u);
        }

        internal void dodajPomiar(int id, Pomiar p)
        {
            drzewo.AddMeasure(id, p);
        }

        internal void dodajPomiar(int ix, DateTime t, Decimal v)
        {
            drzewo.AddMeasure(ix, t, v);
        }

        //usun -- nie istnieje
        internal void usunUrzadzenie(Urzadzenie_Model u) { 
            throw new NotImplementedException();
        }

        //szukaj
        //zwraca listę urządzeń znajdujących się w zadanym prostokącie
        internal List<Urzadzenie_Model> szukaj(Rectangle rect) {
            return drzewo.SearchBy(rect);
        }

        //zwraca urządzenie w podanym punkcie
        internal Urzadzenie_Model szukaj(Decimal x, Decimal y) { 
            return drzewo.SearchBy(x, y);
        }

        //zwraca liczbę pomiarów i agregat powierzchniowy (z czego?)
        internal (Decimal, Decimal) szukajAgregatuPowierzchniowego(Rectangle rect) { 
            return drzewo.FindSpaceAggregate(rect);
        }

        //zwraca agregat czasowy
        public Decimal szukajAgregatuCzasowego(Decimal x, Decimal y) {
            return drzewo.GetTimeAggregate(x, y);
        }

        public void obliczAgregaty()
        {
            drzewo.SpaceAggregate();
        }
    }
}
