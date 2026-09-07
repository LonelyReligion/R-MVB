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
using CsvHelper;
using System.Globalization;
using System.Dynamic;

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

        internal void zapiszEncje(string v)
        {
            using (var ctx = new Kontekst())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.LazyLoadingEnabled = false;
                ctx.Configuration.ProxyCreationEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;
            }
            zapiszUrzadzenia(v);
            zapiszWersje(v);
            zapiszPomiary(v);
            zapiszWersjePomiary(v);
        }

        internal void zapiszUrzadzenia(string v) {
            using (var ctx = new Kontekst())
            {
                List<Urzadzenie> urzadzenia = ctx.Urzadzenia.ToList();

                using (var writer = new StreamWriter(Path.Combine(v, "Urzadzenia.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(urzadzenia);
                }
            }
        }

        internal void zapiszWersje(string v)
        {
            using (var ctx = new Kontekst())
            {
                List<Wersja> wersje = ctx.Wersje.ToList();

                using (var writer = new StreamWriter(Path.Combine(v, "Wersje.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(wersje);
                }
            }
        }

        internal void zapiszPomiary(string v)
        {
            using (var ctx = new Kontekst())
            {
                List<Pomiar> pomiary = ctx.Pomiary.ToList();

                using (var writer = new StreamWriter(Path.Combine(v, "Pomiary.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(pomiary);
                }
            }
        }

        internal void zapiszWersjePomiary(string v)
        {
            using (var ctx = new Kontekst())
            {
                var WersjePomiary = new List<dynamic>();
                foreach (Wersja w in ctx.Wersje.ToList())
                {
                    foreach (Pomiar p in w.Pomiary.ToList())
                    {
                        dynamic obiekt = new ExpandoObject();
                        obiekt.IDUrzadzenia = w.UrzadzenieID;
                        obiekt.IDWersji = w.WersjaID;
                        obiekt.IDPomiaru = p.PomiarID;

                        WersjePomiary.Add(obiekt);
                    }
                }

                using (var writer = new StreamWriter(Path.Combine(v, "WersjePomiary.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(WersjePomiary);
                }
            }
        }

        internal void odczytajEncje() 
        {
            throw new NotImplementedException();
        }

        internal List<Urzadzenie> odczytajUrzadzenia() 
        { 
            throw new NotImplementedException();
        }

        internal List<Urzadzenie> odczytajWersje()
        {
            throw new NotImplementedException();
        }

        internal List<Urzadzenie> odczytajPomiary()
        {
            throw new NotImplementedException();
        }

        internal List<dynamic> odczytajWersjePomiary()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _repo.Reset();
            _rmvb.Reset();
        }
    }
}
