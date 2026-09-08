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
using System.Reflection.Emit;

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
            zapiszPomiary(v);
            zapiszUrzadzeniaPomiary(v);
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

        internal void zapiszUrzadzeniaPomiary(string v)

        {
            using (var ctx = new Kontekst())
            {
                //tu poprawwic
                var UrzadzeniaPomiary = new List<dynamic>();
                foreach (Wersja w in ctx.Wersje.ToList())
                {
                    foreach (Pomiar p in w.Pomiary.ToList())
                    {
                        dynamic obiekt = new ExpandoObject();
                        obiekt.IDUrzadzenia = w.UrzadzenieID;
                        obiekt.IDPomiaru = p.PomiarID;

                        UrzadzeniaPomiary.Add(obiekt);
                    }
                }

                using (var writer = new StreamWriter(Path.Combine(v, "UrzadzeniaPomiary.csv")))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(UrzadzeniaPomiary.Distinct().ToList());
                }
            }
        }

        internal bool odczytajEncje() 
        {
            try
            {
                List<Urzadzenie> urzadzenia = odczytajUrzadzenia();
                List<Pomiar> pomiary = odczytajPomiary();
                List<dynamic> urzadzeniaPomiary = odczytajUrzadzeniaPomiary();

/*                foreach(var urzadzenie in urzadzenia)
                    dodajUrzadzenie(urzadzenie);

                foreach (var pomiar in pomiary)
                {
                    Wersja wersja = new Wersja(pomiar.WersjeUrzadzenia.First().UrzadzenieID, _pamiec.zwrocRMVB(), pomiar.dtpomiaru);

                    _pamiec.dodajWersje(wersja);
                    _pamiec.dodajPomiar(urzadzenie.UrzadzenieID, pomiar, wersja);
                }*/

                return true;
            }
            catch (System.IO.FileNotFoundException) {
                Console.WriteLine("Brakuje plików. Upewnij się, że w folderze znajduje się plik Urzadzenia.csv i Pomiary.csv.");
            }
            catch (Exception ex)
            {
                //do przetestowania
                Console.WriteLine("Załączono plik z danymi w niewłaściwych formacie.");
            }
            return false;
        }

        internal List<Urzadzenie> odczytajUrzadzenia() 
        {
            using (var reader = new StreamReader("..\\..\\..\\Pliki wejściowe\\Urzadzenia.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<Urzadzenie>().ToList();
            }
        }

        internal List<Pomiar> odczytajPomiary()
        {
            using (var reader = new StreamReader("..\\..\\..\\Pliki wejściowe\\Pomiary.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<Pomiar>().ToList();
            }
        }

        internal List<dynamic> odczytajUrzadzeniaPomiary() {
            using (var reader = new StreamReader("..\\..\\..\\Pliki wejściowe\\UrzadzeniaPomiary.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<dynamic>().ToList();
            }
        }

        public void Reset()
        {
            _repo.Reset();
            _rmvb.Reset();
        }
    }
}
