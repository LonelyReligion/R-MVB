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
        double Pversion = 1/3;
        Repo Repo;

        private DeskryptorKorzenia dk;

        public Drzewo(Repo repo) {
            dk = new DeskryptorKorzenia(repo);
            this.Repo = repo;
        }

        public void wypiszDrzewo() {
            dk.wypisz();
        }

        public void dodajUrzadzenie(Urzadzenie u) { 
            dk.dodaj(u);
        }
    }
}
