using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    internal class Drzewo
    {
        double Pversion = 1.0/3;
        double Psvu = 1.0/3;
        double Psvo = 5.0/6;

        Repo Repo;

        private List<DeskryptorKorzenia> desk = new List<DeskryptorKorzenia>();
        Korzen dk;

        public Drzewo(Repo repo) {
            dk = new Korzen(repo, Pversion, Psvu, Psvo);
            desk.Add(new DeskryptorKorzenia(DateTime.Now, DateTime.MaxValue, dk));
            this.Repo = repo;
        }

        public void wypiszDrzewo() {
            dk.wypisz();
        }

        public void dodajUrzadzenie(Urzadzenie u) { 
            dk.dodaj(u);
        }

        internal void usunUrzadzenie(Urzadzenie testowe2)
        {
            dk.usun(testowe2);
        }

        //szukaj id i wersji
        internal Urzadzenie szukaj(int id, int v)
        {
            return dk.szukaj(id, v);
        }

        //szukaj wersji aktualnej w danym momencie
        internal Urzadzenie szukaj(int id, DateTime dt) { 
            return dk.szukaj(id, dt);
        }

        //szukaj ostatniej wersji
        internal Urzadzenie szukaj(int id) { 
            return dk.szukaj(id);
        }

        //zwraca wersje z danego skonczonego przedzialu czasowego
        internal List<Urzadzenie> szukaj(DateTime poczatek, DateTime koniec) {
            return dk.szukaj(poczatek, koniec); //niezaimplementowane
        }
    }
}
