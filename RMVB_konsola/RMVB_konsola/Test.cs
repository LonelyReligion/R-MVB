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
using System.Diagnostics.Metrics;

//singleton?
namespace RMVB_konsola
{
    //singleton?
    internal class Test
    {
        private Repo repo;
        private Kontekst ctx;
        private RMVB rmvb;
        private Generatory generator;

        internal Stopwatch sw;
        internal Random rnd = new Random();

        public Test(Repo r, Kontekst c, RMVB rmvb, Generatory gen)
        {
            repo = r;
            ctx = c;
            this.rmvb = rmvb;
            this.generator = gen;
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
            testProstokat(ileRazy); //przerobic na rozne granice
            testAgregatyCzasowe(ileRazy);
            testAgregatyPowierzchniowe(); //niezaimplementowane
        }

        private void testAgregatyPowierzchniowe()
        {
/*            decimal resultDB = 0;
            decimal resultQt = 0;
            decimal resultDBRTree = 0;
            decimal resultRTree = 0;

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            int ile = 0;
            int cnt_1 = 0;
            for (int i = 0; i < 10; i++)
            {
                resultDB = 0;
                ile = 0;

                int cnt = 0;
                List<int> ids = new List<int>();
                foreach (Urzadzenie u in ctx.Urzadzenia)
                {
                    bool a = u.Dlugosc < y1;
                    bool b = u.Dlugosc >= y2;
                    bool c = u.Szerokosc <= x1;
                    bool d = u.Szerokosc > x2;

                    if (a | b | c | d)
                        ;
                    else
                        ids.Add(u.UrzadzenieID);
                }
                String Out = "(";
                foreach (Pomiar p in ctx.Pomiary)
                {
                    if (p.UrzadzenieID != null && ids.Contains((int)p.UrzadzenieID) && p.dtpomiaru > new DateTime(2024, 7, 18, 0, 0, 0))
                    {
                        ile++;
                        resultDB += p.Wartosc;
                        Out += p.Wartosc + "+";
                    }
                }
                Console.WriteLine(Out + ")/" + ile);
                if (ile != 0)
                    resultDB /= ile;
                else
                    resultDB = 0;
                cnt_1 = cnt;
                Console.WriteLine("===========");
            }
            long wynik = sw.ElapsedMilliseconds;
            Console.WriteLine("(recznie) Policzona na podstawie " + ile.ToString() + " elementow")
                ;
            sw = Stopwatch.StartNew();
            int cnt_4 = 0;
            for (int i = 0; i < 10; i++)
            {
                resultQt = qt.zwrocSrednia(y1, y2, x1, x2);
            }
            long wynik2 = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
            int cnt_1R = 0;
            for (int i = 0; i < 10; i++)
            {
                resultDBRTree = 0;
                int ilosc = 0;

                int cntR = 0;
                List<int> idiki = new List<int>();
                foreach (Device u in ctx.Devices)
                {
                    bool a = u.y < y1;
                    bool b = u.y >= y2;
                    bool c = u.x <= x1;
                    bool d = u.x > x2;

                    if (a | b | c | d)
                        ;
                    else
                        idiki.Add(u.DeviceId);
                }

                foreach (Measurement p in ctx.Measurements)
                {
                    if (p.DeviceId != null && idiki.Contains((int)p.DeviceId))
                    {
                        ilosc++;
                        resultDBRTree += p.rvalue;
                    }
                }
                if (ilosc != 0)
                    resultDBRTree /= ilosc;
                else
                    resultDBRTree = 0;
                cnt_1R = cntR;
            }
            long wynikDBRTree = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
            Rectangle searchRect = new Rectangle(x1, y1, x2, y2);
            int cnt_r = 0;
            for (int i = 0; i < 10; i++)
            {
                resultRTree = rtree.FindSpaceAggregate(searchRect);
            }
            long wynik3 = sw.ElapsedMilliseconds;

            Console.WriteLine("");
            Console.WriteLine("Szukanie agregatu przestrzennego dla obszaru: xMin(" + x1 + "), " + "yMin(" + y1 + "), " + "xMax(" + x2 + "), " + "yMax(" + y2 + "), ");
            Console.WriteLine("WARTOŚCI: Recznie Qt: " + resultDB + " vs " + "Quadtree: " + resultQt + " vs " + " RTree DB: " + resultDBRTree + " vs " + "Rtree: " + resultRTree);
            Console.WriteLine("CZASY:    Recznie Qt: " + wynik + " vs " + "Quadtree: " + wynik2 + " vs " + "RTree DB " + wynikDBRTree + " vs " + "Rtree: " + wynik3);
            Console.WriteLine(qt.licz_elementy().ToString() + " " + ctx.Urzadzenia.Count().ToString());
        }*/
    }

        //wyszukuje agregat czasowy 
        private void testAgregatyCzasowe(int ileRazy)
        {
            //losowanie ze zwracaniem
            //for(int i = 0; i < ileRazy; i++)
                (Decimal x, Decimal y) = generator.wylosujWspolrzedne();

            decimal wynikBD = 0;
            decimal wynikR = 0;

            Stopwatch sw;
            int cnt_1 = 0;

            sw = Stopwatch.StartNew();
            int liczba = 0;
            int id = -1;
            for (int i = 0; i < ileRazy; i++)
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
                    List<Pomiar> pomiary =  ctx.Pomiary
                                            .AsNoTracking()
                                            .Where(p => p.WersjeUrzadzenia.FirstOrDefault().UrzadzenieID == id)
                                            .Where(p => p.dtpomiaru > new DateTime(2024, 7, 18, 0, 0, 0)) //zparametryzowac
                                            .ToList();
                    liczba += pomiary.Count;
                    foreach (Pomiar p in pomiary)  wynikBD += p.Wartosc;

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
            for (int i = 0; i < ileRazy; i++)
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
            List<Rectangle> searchRect = new List<Rectangle>();
            for(int i = 0; i < ileRazy; i++)
                searchRect.Add(generator.generujProstokat()); //powinno wyjsc 2 przy det. 7 urzadzeniach

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            List<int> cnt_1 = new List<int>();
            for (int i = 0; i < ileRazy; i++)
            {
                Rectangle rect = searchRect[i];
                cnt_1.Add(ctx.Urzadzenia
                .AsNoTracking()
                //intersects
                .Where(u => rect.XMin < u.Dlugosc)
                .Where(u => rect.XMax > u.Dlugosc)
                .Where(u => rect.YMin < u.Szerokosc)
                .Where(u => rect.YMax > u.Szerokosc)
                //contains
                .Where(u => rect.XMin <= u.Dlugosc)
                .Where(u => rect.YMin <= u.Szerokosc)
                .Where(u => rect.XMax >= u.Dlugosc)
                .Where(u => rect.YMax >= u.Szerokosc)
                .Count());
            }
            long wynik = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
                

            List<int> cnt_r = new List<int>();
            for (int i = 0; i < ileRazy; i++)
            {
                cnt_r.Add(rmvb.szukaj(searchRect[i]).Count());
            }
            long wynik3 = sw.ElapsedMilliseconds;

            Console.WriteLine("**********************************");
            for (int i = 0; i < ileRazy; i++)
            {
                Console.WriteLine("Prostokat: " + searchRect[i].XMin + " " + searchRect[i].XMax + "(x) " + searchRect[i].YMin + " " + searchRect[i].YMax + "(y)");
                Console.WriteLine("Znaleziono " + cnt_r[i].ToString() + "(rt) " + cnt_1[i].ToString() + "(zapytanie w bazie)");
                Console.WriteLine("**********************************");
            }

            Console.WriteLine("RMVB: " + wynik3 + " vs " + "Recznie: " + wynik);
        }


        //wyszukuje urządzenie w podanym punkcie x ileRazy
        private void testPunkt(int ileRazy) { 
            throw new NotImplementedException();
        }

        //wyszukiwanie losowego urządzenia po dacie i id x ileRazy
        public void testDataId(int ileRazy)
        {
            List<Wersja> szukane_wersje = generator.wylosujWersje(ileRazy);

            Wersja? szukana = null;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++) {
                int id = szukane_wersje[i].UrzadzenieID;
                DateTime dt = szukane_wersje[i].dataOstatniejModyfikacji;
                szukana = ctx.Wersje
                    .AsNoTracking()
                    .Where(u => u.dataOstatniejModyfikacji <= dt)
                    .Where(u => u.dataWygasniecia >= dt)
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
                int losowe_urzadzenie_id = repo.pobierzUrzadzenia().ElementAt(rnd.Next(repo.pobierzUrzadzenia().Count - 1)).Value.UrzadzenieID;
                int losowa_wersja_id = repo.pobierzUrzadzeniaWersje().ElementAt(losowe_urzadzenie_id).Value.ElementAt(rnd.Next(repo.pobierzUrzadzeniaWersje().ElementAt(losowe_urzadzenie_id).Value.Count - 1)).WersjaID;
                szukane_id_v.Add((losowe_urzadzenie_id, losowa_wersja_id));
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
