using Microsoft.Win32;
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

        private List<DeskryptorKorzenia> desk = new List<DeskryptorKorzenia>(); // "List of tree descriptors. Descriptors for all roots in the tree are connected in a list(or other structures) according to growing, separable life spans."

        public Drzewo(Repo repo) {
            Korzen k = new Korzen(repo, Pversion, Psvu, Psvo);
            desk.Add(new DeskryptorKorzenia(DateTime.Now, DateTime.MaxValue, k));
            this.Repo = repo;
        }

        public void wypiszDrzewo() {
            foreach (DeskryptorKorzenia dk in desk)
            {
                dk.korzen.wypisz();
            }
        }

        public void dodajUrzadzenie(Urzadzenie u) {
            //w jakis sposob (na podstawie dat) wybieramy korzen
            var dk = desk[0].korzen;
            dk.dodaj(u);
        }

        internal void usunUrzadzenie(Urzadzenie testowe2)
        {
            var dk = desk[0].korzen;
            dk.usun(testowe2);
        }

        //szukaj id i wersji
        internal Urzadzenie szukaj(int id, int v)
        {
            var dk = desk[0].korzen;
            return dk.szukaj(id, v);
        }

        //szukaj wersji aktualnej w danym momencie
        internal Urzadzenie szukaj(int id, DateTime dt) {
            var dk = desk[0].korzen;
            return dk.szukaj(id, dt);
        }

        //szukaj ostatniej wersji
        internal Urzadzenie szukaj(int id) {
            var dk = desk[0].korzen;
            return dk.szukaj(id);
        }

        //zwraca wersje z danego skonczonego przedzialu czasowego
        internal List<Urzadzenie> szukaj(DateTime poczatek, DateTime koniec) {
            var dk = desk[0].korzen;
            return dk.szukaj(poczatek, koniec); 
        }
    }
}
