using Microsoft.VisualBasic;
using RMVB_konsola.Indeks.MVB;
using RMVB_konsola.Indeks.R;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.Indeks
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
            alfa.dodajPomiar(p);

            using (var ctx = new Kontekst())
            {
                ctx.Wersje.Attach(alfa);
                ctx.Entry(alfa).Collection(x => x.Pomiary).Load();
                ctx.Entry(alfa).State = EntityState.Modified;

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

        // zwracamy srednia z okresu czasu z pomiarow urzadzen znajdujacych sie na podanym obszarze
        internal (int,int, decimal) zwrocLiczbeUrzadzenPomiarowSrednia(Rectangle prostokat, DateTime poczatek, DateTime koniec) {
            List<Urzadzenie> szukane = R.szukaj(prostokat);
            List<int> ids = szukane.Select(u=>u.UrzadzenieID).ToList();
            int liczba_urzadzen = szukane.Count;

            decimal suma = 0;
            int liczba_pomiarow = 0;

            if (poczatek == DateTime.MinValue)
            {
                foreach (var urzadzenie in szukane)//moze parallel?
                {
                    (decimal srednia, int liczba) = MVB.szukaj(urzadzenie.UrzadzenieID, koniec).PobierzSredniaIliczbe();
                    suma += srednia * liczba;
                    liczba_pomiarow += liczba;
                }

                
            }
            else {
                List<Wersja> wersje = MVB.szukaj(poczatek, koniec); //tu sie wersje beda powtarzac, chodzi nam o ta ostatnia z kazdego urzadzenia
                wersje = wersje
                        .Where(p => ids.Contains(p.UrzadzenieID))
                        .GroupBy(x => x.UrzadzenieID)
                        .Select(g => g.MaxBy(x => x.WersjaID))
                        .OrderBy(w => w.UrzadzenieID)
                        .ToList();

                foreach (var wersja in wersje) {
                    foreach (var pomiar in wersja.Pomiary) 
                    {
                        if (pomiar.dtpomiaru >= poczatek && pomiar.dtpomiaru < koniec)
                        {
                            suma += pomiar.Wartosc;
                            liczba_pomiarow++;
                        }
                    }
                }

            }

            if (liczba_pomiarow != 0)
                return (liczba_urzadzen, liczba_pomiarow, (suma / liczba_pomiarow));
            else
                return (liczba_urzadzen, 0, 0);
        }

        internal void zapiszMVB(string v)
        {
            List<string> linijki = MVB.drukujDrzewo();
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(v, "MVB.txt")))
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
