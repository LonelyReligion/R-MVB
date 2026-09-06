using Symulacja_strumieni.model;
using Symulacja_strumieni.rmvb.mvb;
using Symulacja_strumieni.rmvb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Symulacja_strumieni;

namespace PrezentacjaRownoleglosci.baza
{
    public class Pamiec : Konsument
    {
        RMVB _rmvb;
        Repo _repo;

        public Pamiec(BlockingCollection<object> kolekcja) : base(kolekcja)
        {
            _rmvb = new RMVB();
            _repo = _rmvb.zwrocRepo();
        }

        public RMVB zwrocRMVB()
        {
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
            List<Task> ts = new List<Task>();
            ts.Add(Task.Run(() => _repo.saveDevice(u)));
            ts.Add(Task.Run(() => _rmvb.dodajUrzadzenie(u)));
            Task.WaitAll(ts.ToArray());
        }

        internal void dodajWersje(Wersja w)
        {
            List<Task> ts = new List<Task>();
            ts.Add(Task.Run(() => _repo.saveVersion(w)));
            ts.Add(Task.Run(() => _rmvb.dodajWersje(w)));
            Task.WaitAll(ts.ToArray());
        }

        internal void dodajPomiar(int UrzadzenieID, Pomiar p, Wersja alfa)
        {
            //tu tez jakies zrownoleglenie?
            alfa.dodajPomiar(p);
            _repo.saveMeasurement(p, alfa);
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

        public override void Konsumuj()
        {
            //jakos inaczej ofc, ale to nie teraz
            foreach (var last in kolekcja.GetConsumingEnumerable())
            {
                try
                {
                    Urzadzenie urzadzenie = (Urzadzenie)last;
                    this.dodajUrzadzenie(urzadzenie);
                    Console.WriteLine("Odebrano urządzenie o id " + urzadzenie.UrzadzenieID + ".");
                }
                catch
                {
                    try
                    {
                        (int id, Pomiar pomiar) = ((int, Pomiar))last;
                        Console.WriteLine("Odebrano pomiar " + pomiar.Wartosc + " st. C przypisany do Urzadzenia o id " + id + ".");
                        Wersja wersja = new Wersja(id, _repo, _rmvb, pomiar.dtpomiaru);
                        this.dodajWersje(wersja);
                        this.dodajPomiar(id, pomiar, wersja);
                    }
                    catch
                    {
                        Console.WriteLine("Nie udalo sie odczytac danych.");
                    }
                }

            }
        }

    }
}
