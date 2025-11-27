using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RMVB_konsola.MVB;
using System.Data;
using System.Collections;
using RMVB_konsola.R;

//singleton?
namespace RMVB_konsola
{
    //singleton?
    internal class Test
    {
        private Repo repo;
        private Kontekst ctx;
        private RMVB rmvb;

        internal Stopwatch sw;
        internal Random rnd = new Random();

        public Test(Repo r, Kontekst c, RMVB rmvb)
        {
            repo = r;
            ctx = c;
            this.rmvb = rmvb;
        }

        public void wykonajTesty(int ileRazy) {
            Console.WriteLine("Poniżej zaprezentowano wyniki przeprowadzonych testów");
            
            Console.WriteLine("Sekcja pierwsza: zapytania realizowane przez MVB");
            Console.WriteLine("# Wyszukiwanie po dacie i id");
            testDataId(ileRazy);


            Console.WriteLine("\n# Wyszukiwanie po id");
            testId(ileRazy);


            Console.WriteLine("\n# Wyszukiwanie po id i wersji");
            testIdV(ileRazy);


            Console.WriteLine("\n# Wyszukiwanie po dacie i dacie");
            testDataData(ileRazy);
            Console.WriteLine("\n");

            Console.WriteLine("Sekcja druga: zapytania realizowane przez R");
            Console.WriteLine("# Wyszukiwanie urzadzen znajdujacych sie w losowym prostokacie");
            testProstokat(ileRazy);
            testAgregatyCzasowe(); //nieskonczone, czy przerobic na ileRazy?
        }

        private List<Wersja> wylosujWersje(int ile) {
            List<Wersja> szukane_wersje = new List<Wersja>();
            for (int i = 0; i < ile; i++)
            {
                Wersja losowa_wersja = repo.pobierzWersje().ElementAt(rnd.Next(repo.pobierzWersje().Count - 1));
                szukane_wersje.Add(losowa_wersja);
            }
            return szukane_wersje;
        }

        //zwraca wspolrzedne losowego urzadzenia
        private (Decimal, Decimal) wylosujWspolrzedne() { 
            Urzadzenie losowe = repo.pobierzUrzadzenia().ElementAt(rnd.Next(repo.pobierzUrzadzenia().Count - 1)).Value;
            return (losowe.Dlugosc, losowe.Szerokosc);
        }
        private void testAgregatyCzasowe()
        {
            (Decimal x, Decimal y) = wylosujWspolrzedne();

            decimal wynikBD = 0;
            decimal wynikR = 0;

            Stopwatch sw;
            int cnt_1 = 0;

            sw = Stopwatch.StartNew();
            int liczba = 0;
            int id = -1;
            for (int i = 0; i < 10; i++)
            {
                wynikBD = 0;
                liczba = 0;

                id = ctx.Urzadzenia
                    .AsNoTracking()
                    .Where(u => u.Szerokosc == y)
                    .Where(u => u.Dlugosc == x)
                    .First()
                    .UrzadzenieID;

                if (id != -1)
                {
                    foreach (Pomiar m in ctx.Pomiary)
                    {
                        if (m.WersjeUrzadzenia.First().UrzadzenieID == id && m.dtpomiaru > new DateTime(2024, 7, 18, 0, 0, 0))
                        {
                            liczba++;
                            wynikBD += m.Wartosc;
                        }
                        /*else if (m.UrzadzenieID == id && (m.dtpomiaru < new DateTime(2024, 7, 18, 0, 0, 0)))
                        {
                            Console.WriteLine("DB: Pomiar zostal dokonany zbyt dawno temu, aby umiescic go w sredniej! " + m.dtpomiaru);
                        }*/
                    }

                    if (liczba != 0)
                        wynikBD /= liczba;
                    else
                        wynikBD = 0;
                }
                else
                    Console.WriteLine("Urzadzenie o wsp. " + x + " " + y + " nie istnieje w bazie");
            }
            long czasBD = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();

            List<Urzadzenie> resDevices = new List<Urzadzenie>();
            sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                wynikR = rmvb.szukajAgregatuCzasowego(x, y);
            }
            long czas = sw.ElapsedMilliseconds;

            Console.WriteLine("");
            Console.WriteLine("Szukanie agregatu czasowego dla urządzenia o (x, y) = (" + x + ", " + y + ") i id = " + id.ToString());
            Console.WriteLine("WARTOŚCI: Baza: " + wynikBD + " vs " + "Rtree: " + wynikR);
            Console.WriteLine("CZASY: Baza: " + czasBD + " vs " + "Rtree: " + czas);

        }

        //wyszukiwanie urzadzen znajdujacych sie w losowym prostokacie x ileRazy

        private void testProstokat(int ileRazy) {
            Generatory generator_granic = new Generatory();

            Rectangle searchRect = generator_granic.generujProstokatDeterministycznie(); //powinno wyjsc 2 przy det. 7 urzadzeniach

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            int cnt_1 = 0;
            for (int i = 0; i < 10; i++)
            {
                cnt_1 = ctx.Urzadzenia
                .AsNoTracking()
                //intersects
                .Where(u => searchRect.XMin < u.Dlugosc)
                .Where(u => searchRect.XMax > u.Dlugosc)
                .Where(u => searchRect.YMin < u.Szerokosc)
                .Where(u => searchRect.YMax > u.Szerokosc)
                //contains
                .Where(u => searchRect.XMin <= u.Dlugosc)
                .Where(u => searchRect.YMin <= u.Szerokosc)
                .Where(u => searchRect.XMax >= u.Dlugosc)
                .Where(u => searchRect.YMax >= u.Szerokosc)
                .Count();
            }
            long wynik = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
                

            int cnt_r = 0;
            for (int i = 0; i < 10; i++)
            {
                cnt_r = rmvb.szukaj(searchRect).Count();
            }
            long wynik3 = sw.ElapsedMilliseconds;

            Console.WriteLine("Znaleziono " + cnt_r.ToString() + "(rt) " + cnt_1.ToString() + "(zapytanie w bazie)");
            Console.WriteLine("RMVB: " + wynik3 + " vs " + "Recznie: " + wynik);
        }


        //wyszukuje urządzenie w podanym punkcie x ileRazy
        private void testPunkt(int ileRazy) { 
            throw new NotImplementedException();
        }

        //wyszukuje agregat czasowy x ileRazy

        //wyszukiwanie losowego urządzenia po dacie i id x ileRazy
        public void testDataId(int ileRazy)
        {
            List<Wersja> szukane_wersje = wylosujWersje(ileRazy);

            Wersja? szukana = null;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++) {
                int id = szukane_wersje[i].UrzadzenieID;
                DateTime dt = szukane_wersje[i].dataOstatniejModyfikacji;
                szukana = ctx.Wersje
                    .AsNoTracking()
                    .Where(u => u.dataOstatniejModyfikacji <= dt)
                    .Where(u => u.dataWygasniecia > dt)
                    .Where(u => u.UrzadzenieID == id)
                    .FirstOrDefault(); //czasami nie dziala:/
                if (szukana == null)
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
            }
            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza w czasie: " + czas_baza + " ms.");

            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                int id = szukane_wersje[i].UrzadzenieID;
                DateTime dt =  szukane_wersje[i].dataOstatniejModyfikacji;
                szukana = rmvb.szukaj(id, dt);
                if (szukana == null)
                    Console.WriteLine("Uwaga: RMVB nie odnalazlo rekordu.");
            }
            long czas_mvb = sw.ElapsedMilliseconds;
            Console.WriteLine("MVB w czasie: " + czas_mvb + "ms.");
        }

        private List<int> wylosujIdUrzadzen(int ile) {
            List<int> szukane_id = new List<int>();
            for (int i = 0; i < ile; i++)
            {
                int losowe_urzadzenie = repo.pobierzUrzadzenia().ElementAt(rnd.Next(repo.pobierzUrzadzenia().Count - 1)).Key;
                szukane_id.Add(losowe_urzadzenie);
            }
            return szukane_id;
        }

        //wyszukiwanie ostatniej wersji po id
        public void testId(int ileRazy) {
            List<int> szukane_id = wylosujIdUrzadzen(ileRazy);

            Wersja? szukana = null;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++) {
                int id = szukane_id[i];
                szukana = ctx.Wersje
                    .AsNoTracking() //nie uzywamy zbuforowanych (wynikow poprzednich wykonan)
                    .Where(u => u.UrzadzenieID == id)
                    .OrderByDescending(u => u.WersjaID)
                    .FirstOrDefault();
                if (szukana == null)
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
            }
            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza w czasie: " + czas_baza + " ms.");
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                szukana = rmvb.szukaj(szukane_id[i]);
                if (szukana == null)
                    Console.WriteLine("Uwaga: RMVB nie odnalazlo rekordu.");
            }
            long czas_mvb = sw.ElapsedMilliseconds;
            Console.WriteLine("MVB w czasie: " + czas_mvb + " ms.");
            
        }

        //wyszukiwanie po id i wersji
        public void testIdV(int ileRazy) {
            
            List<(int, int)> szukane_id_v = new List<(int, int)>();
            for (int i = 0; i < ileRazy; i++)
            {
                int lodowe_urzadzenie_id = repo.pobierzUrzadzenia().ElementAt(rnd.Next(repo.pobierzUrzadzenia().Count - 1)).Value.UrzadzenieID;
                int losowa_wersja_id = repo.pobierzUrzadzeniaWersje().ElementAt(lodowe_urzadzenie_id).Value.ElementAt(rnd.Next(repo.pobierzUrzadzeniaWersje().ElementAt(lodowe_urzadzenie_id).Value.Count - 1)).WersjaID;
                szukane_id_v.Add((lodowe_urzadzenie_id, losowa_wersja_id));
            }
            //>
            Wersja? szukana = null;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                int id = szukane_id_v[i].Item1;
                int v = szukane_id_v[i].Item2;

                szukana = ctx.Wersje
                .AsNoTracking()
                .FirstOrDefault(u => u.UrzadzenieID == id && u.WersjaID == v);

                if (szukana == null)
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
            }

            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza: " + szukana.UrzadzenieID + "v" + szukana.WersjaID + " w czasie: " + czas_baza + " ms.");

            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                int id = szukane_id_v[i].Item1;
                int v = szukane_id_v[i].Item2;
                szukana = rmvb.szukaj(id, v);
                if (szukana == null)
                    Console.WriteLine("Uwaga: RMVB nie odnalazlo rekordu.");
            }
            long czas_mvb = sw.ElapsedMilliseconds;
            Console.WriteLine("RMVB: " + szukana.UrzadzenieID + "v" + szukana.WersjaID + " w czasie: " + czas_mvb + " ms.");
        }

        public void testDataData(int ileRazy) {
            DateTime poczatek = ctx.Wersje.OrderBy(u=>u.dataOstatniejModyfikacji).FirstOrDefault().dataOstatniejModyfikacji.AddTicks(-10);
            DateTime koniec = ctx.Wersje
                                .OrderByDescending(u => u.dataWygasniecia)
                                .Select(u => u.dataWygasniecia)
                                .First();
            int range = ((TimeSpan)(koniec - poczatek)).Milliseconds;
            List<DateTime> randos = new List<DateTime>();
            randos.Add(poczatek.AddTicks(rnd.Next(range)));

            var szukane_wersje = new List<Wersja>();
            var szukane_wersje_mvb = new List<Wersja>();

            //Console.WriteLine(poczatek.Ticks + "-" + koniec.Ticks);
            sw = Stopwatch.StartNew();
            for(int i = 0; i < ileRazy; i++)
                szukane_wersje = ctx.Wersje.AsNoTracking().Where(u => u.dataOstatniejModyfikacji >= poczatek).Where(u => u.dataWygasniecia < koniec).ToList();
            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza: " + szukane_wersje.Count + " w czasie: " + czas_baza + " ms.");
            
            sw = Stopwatch.StartNew();
            szukane_wersje_mvb = rmvb.szukaj(poczatek, koniec);
            long czas_mvb = sw.ElapsedMilliseconds;
            Console.WriteLine("RMVB: " + szukane_wersje_mvb.Count + " w czasie: " + czas_mvb + " ms.");

            if (szukane_wersje.Count != szukane_wersje_mvb.Count)
            {
                //except nie zadziala
                var nieznalezione = szukane_wersje
                                    .Where(d => !szukane_wersje_mvb.Any(mvb =>
                                        mvb.UrzadzenieID == d.UrzadzenieID &&
                                        mvb.WersjaID == d.WersjaID))
                                    .ToList();
                foreach(var u in nieznalezione)
                    Console.WriteLine(u.UrzadzenieID + "v" + u.WersjaID + " " + u.dataOstatniejModyfikacji.Ticks + "-" + u.dataWygasniecia.Ticks);
            }
        }
    }
}
