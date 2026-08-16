using Symulacja_strumieni.rmvb.mvb;
using Symulacja_strumieni.rmvb.r;
using Symulacja_strumieni.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulacja_strumieni.rmvb
{
    public class RMVB
    {
        private DrzewoMVB MVB;
        private RTreeAdapter R;
        private Repo repo;
        internal RMVB()
        {
            repo = new Repo();
            MVB = new DrzewoMVB(repo, this);
            R = new RTreeAdapter(new RTree(repo));
        }

        internal Repo zwrocRepo() { return repo; }
        internal bool czyUrzadzenieIstnieje(int id) { return repo.czyUrzadzenieIstnieje(id); }
        internal DrzewoMVB zwrocMVB() { return MVB; }
        internal void wypiszMVB()
        {
            foreach (string linijka in MVB.drukujDrzewo())
                Console.WriteLine(linijka);
        }
        //dodaj
        internal void dodajUrzadzenie(Urzadzenie u)
        {
            repo.saveDevice(u);
            R.dodajUrzadzenie(u);
        }

        internal void dodajWersje(Wersja w)
        {
            repo.saveVersion(w);
            MVB.dodajUrzadzenie(w);
        }

        internal void dodajPomiar(int UrzadzenieID, Pomiar p, Wersja alfa)
        {

            using (var ctx = new Kontekst())
            {
                ctx.Wersje.Attach(alfa);
                ctx.Entry(alfa).Collection(x => x.Pomiary).Load();

                alfa.Pomiary.Add(p);
                ctx.Pomiary.Add(p);
                ctx.SaveChanges();
            }

            R.dodajPomiar(UrzadzenieID, p);
        }

        //usun
        internal void usunWersje(Wersja w)
        {
            MVB.usunUrzadzenie(w); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow
            repo.modifyVersion(w);
        }

        //szukaj
        //wyszukiwanie wersji o UrządzenieID równym id i WersjaID równym v
        internal Wersja szukaj(int id, int v)
        {
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
        internal List<Wersja> szukaj(DateTime poczatek, DateTime koniec)
        {
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
            return R.szukaj(x, y);
        }

        //zwraca liczbę pomiarów i agregat czasowy (z czego?)
        internal (List<int> ids, decimal, decimal) szukajAgregatu(Rectangle rect)
        {
            return R.szukajAgregatuPowierzchniowego(rect);
        }

        //zwraca agregat czasowy urzadzenia
        internal decimal szukajAgregatuCzasowego(decimal x, decimal y)
        {
            return R.szukajAgregatuCzasowego(x, y);
        }

        //oblicza agregaty powierzchniowe
        internal void obliczAgregaty()
        {
            R.obliczAgregaty();
        }

        internal void zapiszMVB(string v)
        {
            List<string> linijki = MVB.drukujDrzewo();
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(v, "mvb.txt")))
            {
                foreach (string linijka in linijki)
                    outputFile.WriteLine(linijka);
            }
        }

        public void Reset()
        {
            repo.Reset();
            MVB = new DrzewoMVB(repo, this);
            R = new RTreeAdapter(new RTree(repo));
        }
    }
}
