using Microsoft.Win32;
using RMVB_konsola.R;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    internal class DrzewoMVB
    {
        private double Pversion = 1.0/3;
        private double Psvu = 1.0/3;
        private double Psvo = 5.0/6;

        private TreeRepository Repo;
        private Kontekst ctx;

        private List<DeskryptorKorzenia> desk = new List<DeskryptorKorzenia>(); // "List of tree descriptors. Descriptors for all roots in the tree are connected in a list(or other structures) according to growing, separable life spans."

        internal DrzewoMVB(TreeRepository repo, Kontekst ctx)
        {
            Korzen k = new Korzen(repo, Pversion);

            Wezel.Psvu = Psvu;
            Wezel.Psvo = Psvo;

            desk.Add(new DeskryptorKorzenia(DateTime.Now, DateTime.MaxValue, k));
            this.Repo = repo;
            this.ctx = ctx;
        }

        internal void wypiszDrzewo() {
            foreach (DeskryptorKorzenia dk in desk)
            {
                dk.korzen.wypisz();
            }
        }

        internal int zwrocLiczbeWpisowKorzenia(int nr) {
            return desk[nr].korzen.zwrocLiczbeWpisow();
        }

        internal void dodajUrzadzenie(Wersja u) {
            //w jakis sposob (na podstawie dat) wybieramy korzen
            var dk = desk[0].korzen;
            dk.dodaj(u);
        }

        internal void usunUrzadzenie(Wersja testowe2)
        {
            var dk = desk[0].korzen;
            dk.usun(testowe2);
        }

        //szukaj id i wersji
        internal Wersja szukaj(int id, int v)
        {
            var dk = desk[0].korzen;
            return dk.szukaj(id, v).Item2;
        }

        //szukaj wersji aktualnej w danym momencie
        internal Wersja szukaj(int id, DateTime dt) {
            var dk = desk[0].korzen;
            return dk.szukaj(id, dt);
        }

        //szukaj ostatniej wersji
        internal Wersja szukaj(int id) {
            var dk = desk[0].korzen;
            return dk.szukaj(id);
        }

        //zwraca wersje z danego skonczonego przedzialu czasowego
        internal List<Wersja> szukaj(DateTime poczatek, DateTime koniec) {
            var dk = desk[0].korzen;
            return dk.szukaj(poczatek, koniec); 
        }
    }
}
