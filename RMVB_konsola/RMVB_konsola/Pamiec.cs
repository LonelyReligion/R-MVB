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
using System.Configuration;

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
                    csv.Context.RegisterClassMap<UrzadzenieMap>();
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
                    var unikatoweWartosci = UrzadzeniaPomiary.GroupBy(wiersz => wiersz.IDPomiaru).Select(group => group.First()); 
                    csv.WriteRecords(unikatoweWartosci);
                }
            }
        }

        internal bool odczytajEncje() 
        {
            try
            {
                List<Urzadzenie> urzadzenia = odczytajUrzadzenia();
                List<Pomiar> pomiary = odczytajPomiary();
                Dictionary<int, int> urzadzeniaPomiary = odczytajUrzadzeniaPomiary();

                foreach (var urzadzenie in urzadzenia)
                    dodajUrzadzenie(urzadzenie);

                foreach (var pomiar in pomiary)
                {
                    Wersja wersja = new Wersja(urzadzeniaPomiary[pomiar.PomiarID], _rmvb, pomiar.dtpomiaru);
                    dodajWersje(wersja);
                    dodajPomiar(urzadzeniaPomiary[pomiar.PomiarID], pomiar, wersja);
                }

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
                csv.Context.RegisterClassMap<UrzadzenieMap>();
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

        //klucz: id pomiaru
        //wartosc: id urzadzenia
        internal Dictionary<int, int> odczytajUrzadzeniaPomiary() {
            List<dynamic> lista = new List<dynamic>();
            using (var reader = new StreamReader("..\\..\\..\\Pliki wejściowe\\UrzadzeniaPomiary.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                lista = csv.GetRecords<dynamic>().ToList();
            }

            Dictionary<int, int> slownik = new Dictionary<int, int>();
            foreach (var obiekt in lista) 
            {
                int IDUrzadzenia = Convert.ToInt32(obiekt.IDUrzadzenia);
                int IDPomiaru = Convert.ToInt32(obiekt.IDPomiaru);

                if (!slownik.ContainsKey(IDPomiaru))
                {
                    slownik[IDPomiaru] = IDUrzadzenia;
                }
                else 
                {
                    Console.WriteLine("UWAGA: Mamy powtarzajace sie pomiary. Działanie programu może być nieprawidłowe.");
                }
            }
            return slownik;
        }

        public void Reset()
        {
            _repo.Reset();
            _rmvb.Reset();
        }

        public bool zaladujZmienne(ref string sciezkaFolderuWyjsciowego, ref int liczbaUrzadzen, ref bool generujemy) {
            sciezkaFolderuWyjsciowego = ConfigurationManager.AppSettings.Get("sciezka_folderu_wyjsciowego");

            Directory.CreateDirectory(sciezkaFolderuWyjsciowego);
            if (!Directory.Exists(sciezkaFolderuWyjsciowego))
            {
                Console.WriteLine("Podana ścieżka jest niepoprawna.");
                return false;
            }
            Console.WriteLine("Pliki wyjściowe znajdziesz pod adresem: " + Path.GetFullPath(sciezkaFolderuWyjsciowego));

            string generujemyStr = ConfigurationManager.AppSettings.Get("generujemy").Trim();
            if (generujemyStr == "false" || generujemyStr == "False")
            {
                generujemy = false;
            }
            else if (generujemyStr == "true" || generujemyStr == "True")
            {
                generujemy = true;
                string liczbaUrzadzenStr = ConfigurationManager.AppSettings.Get("liczba_urzadzen");
                try
                {
                    liczbaUrzadzen = int.Parse(liczbaUrzadzenStr);
                    Generatory.liczba_urzadzen = liczbaUrzadzen;
                }
                catch
                {
                    Console.WriteLine("Podana liczba urządzeń nie jest liczbą całkowitą.");
                    Console.WriteLine("Podaj poprawną liczbę urządzeń id spróbuj ponownie.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Wartość atrubutu 'generujemy' jest nieprawidłowa. Atrybut przyjmuje wartości: true, false, True, False.");
                Console.WriteLine("Podaj poprawną wartość atrubutu i spróbuj ponownie.");
                return false;
            }

            double granicaPrzezywalnosci = 0;
            string granicaPrzezywalnosciStr = ConfigurationManager.AppSettings.Get("granica_przezywalnosci");
            string minimalnaLiczbaUrzadzenWKorzeniu = ConfigurationManager.AppSettings.Get("min_urzadzen_korzen");

            CultureInfo kultura = CultureInfo.CreateSpecificCulture("pl-PL");
            try
            {
                granicaPrzezywalnosci = Double.Parse(granicaPrzezywalnosciStr, kultura);
                Korzen.granica_przezywalnosci = (decimal)granicaPrzezywalnosci;
            }
            catch
            {
                Console.WriteLine("Podana granica przeżywalności urządzeń nie jest poprawna.");
                Console.WriteLine("Czy użyłeś/aś kropki (.) zamiast przecinka (,)?");
                Console.WriteLine("Podaj poprawną granicę przeżywalności id spróbuj ponownie.");
                return false;
            }

            try
            {
                int minimalnaLiczbaUrzadzenWKorzeniu_int = int.Parse(minimalnaLiczbaUrzadzenWKorzeniu);
                Korzen.min_urzadzen_korzen = minimalnaLiczbaUrzadzenWKorzeniu_int;
            }
            catch
            {
                Console.WriteLine("Minimalna liczba urządzeń w korzeniu nie jest liczbą całkowitą.");
                Console.WriteLine("Podaj poprawną liczbę urządzeń w korzeniu id spróbuj ponownie.");
                return false;
            }
            return true;
        }
    }
}
