using RMVB_konsola.MVB;
using RMVB_konsola.R;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    internal class RMVB
    {
        private Kontekst ctx;
        private DrzewoMVB MVB;
        private RTreeAdapter R;
        private Repo repo;
        internal RMVB(Kontekst ctx) {
            this.ctx = ctx;
            repo = new Repo();
            MVB = new DrzewoMVB(repo, ctx);
            R = new RTreeAdapter(new RTree(repo, ctx));
        }

        internal Repo zwrocRepo() { return repo; }
        internal bool czyUrzadzenieIstnieje(int id) { return repo.czyUrzadzenieIstnieje(id); }
        internal DrzewoMVB zwrocMVB() { return MVB; }
        internal void wypiszMVB() { MVB.wypiszDrzewo(); }
        //dodaj
        internal void dodajUrzadzenie(Urzadzenie u) {
            R.dodajUrzadzenie(u);
            repo.saveDevice(u);
        }

        internal void dodajWersje(Wersja w) {
            MVB.dodajUrzadzenie(w);
            repo.saveVersion(w);
        }

        internal void dodajPomiar(int UrzadzenieID, Pomiar p) {
            ctx.Pomiary.Add(p);
            R.dodajPomiar(UrzadzenieID, p);
            ctx.SaveChanges();//?
        }

        //usun
        internal void usunWersje(Wersja w) {
            MVB.dodajUrzadzenie(w); //musi zostac zapisana najpierw
            MVB.usunUrzadzenie(w); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow
            repo.saveVersion(w);
        }

        //szukaj
        //wyszukiwanie wersji o UrządzenieID równym id i WersjaID równym v
        internal Wersja szukaj(int id, int v) { 
            return MVB.szukaj(id, v);
        }

        //wyszukiwanie wersji urządzenia o UrzadzenieID aktualnej w chwili dt
        internal Wersja szukaj(int id, DateTime dt)
        {
            return MVB.szukaj(id, dt);
        }

        //wyszukiwanie ostatniej wersji o UrzadzenieID równym id
        internal Wersja szukaj(int id)
        {
            return MVB.szukaj(id);
        }

        //wyszukiwanie wersji aktualnych w podanym przedziale czasowym
        internal List<Wersja> szukaj(DateTime poczatek, DateTime koniec) { 
            return MVB.szukaj(poczatek, koniec);
        }

        //zwraca listę urządzeń znajdujących się w zadanym prostokącie
        internal List<Urzadzenie> szukaj(Rectangle rect)
        {
            return R.szukaj(rect);
        }

        //zwraca urządzenie w podanym punkcie
        internal Urzadzenie szukaj(decimal x, decimal y)
        {
            return R.szukaj((decimal)x, (decimal)y);
        }

        //zwraca liczbę pomiarów i agregat czasowy (z czego?)
        internal (decimal, decimal) szukajAgregatu(Rectangle rect)
        {
            return szukajAgregatu(rect);
        }

        //zwraca agregat czasowy urzadzenia
        internal decimal szukajAgregatuCzasowego(decimal x, decimal y) {
            return szukajAgregatuCzasowego(x, y);
        }
    }
}
