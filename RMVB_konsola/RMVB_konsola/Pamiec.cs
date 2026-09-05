using RMVB_konsola.baza;
using RMVB_konsola.Indeks.MVB;
using RMVB_konsola.Indeks.R;
using RMVB_konsola.Indeks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    internal class Pamiec
    {
        RMVB _rmvb = new RMVB();
        Repo _repo;

        public Pamiec() { 
            _repo = _rmvb.zwrocRepo();
        }

        public RMVB zwrocRMVB() {
            return _rmvb;
        }

        internal Repo zwrocRepo() { return _repo; }
        internal bool czyUrzadzenieIstnieje(int id) { return _repo.czyUrzadzenieIstnieje(id); }
        internal DrzewoMVB zwrocMVB() { return _rmvb.zwrocMVB(); }
        internal void wypiszMVB()
        {
            foreach (string linijka in zwrocMVB().drukujDrzewo())
                Console.WriteLine(linijka);
        }

        //dodaj
        internal void dodajUrzadzenie(Urzadzenie u)
        {
            _repo.saveDevice(u);
            _rmvb.dodajUrzadzenie(u);
        }

        internal void dodajWersje(Wersja w)
        {
            _repo.saveVersion(w);
            _rmvb.dodajWersje(w);
        }

        internal void dodajPomiar(int UrzadzenieID, Pomiar p, Wersja alfa)
        {
            alfa.dodajPomiar(p);
            _repo.saveMeasurement(UrzadzenieID, p, alfa);
            _rmvb.dodajPomiar(UrzadzenieID, p);
        }

        //usun
        internal void usunWersje(Wersja w)
        {
            _rmvb.usunWersje(w); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow
            _repo.modifyVersion(w);
        }

        internal void zapiszMVB(string v)
        {
            List<string> linijki = _rmvb.drukujDrzewo();
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(v, "MVB.txt")))
            {
                foreach (string linijka in linijki)
                    outputFile.WriteLine(linijka);
            }
        }

        public void Reset()
        {
            _repo.Reset();
            _rmvb.Reset();
        }
    }
}
