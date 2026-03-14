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

//singleton, test bedzie wykonywany jednowatkowo stąd brak dodatkowego zabezpieczenia
namespace RMVB_konsola
{
    internal class Test
    {
        public static Repo repo;
        public static Kontekst ctx;
        public static RMVB rmvb;
        public static Generatory generator;
        public static Test instancja;

        private static List<string> wyniki;
        private static List<string> bledy;

        internal Stopwatch sw;
        internal Random rnd = new Random();
        
        public static Test pobierzInstancje() {
            if (instancja == null)
                return new Test();
            else
                return instancja;
        }

        private Test()
        {
        }

        public bool wykonajTesty(int ileRazy) {
            wyniki = new List<string>();
            bledy = new List<string>();

            wyniki.Add("Liczba powtórzeń każdego zapytania: " + ileRazy);
            wyniki.Add("Drzewo realizujące zapytanie | Rodzaj zapytania | Czas wykonania w bazie (ms.) | Czas wykonania za pomocą drzewa (ms.)");
            
            bool blad = false;
            
            
            Console.WriteLine("Poniżej zaprezentowano wyniki przeprowadzonych testów");
            
            Console.WriteLine("Sekcja pierwsza: zapytania realizowane przez MVB");
            Console.WriteLine("# Wyszukiwanie po dacie i id");
            blad = (blad == true) ? true : testDataId(ileRazy);

            Console.WriteLine("\n# Wyszukiwanie po id");
            bool blad1 = testId(ileRazy);
            blad = (blad == true) ? true : blad1;


            Console.WriteLine("\n# Wyszukiwanie po id i wersji");
            bool blad2 = testIdV(ileRazy);
            blad = (blad == true) ? true : blad2;

            Console.WriteLine("\n# Wyszukiwanie po dacie i dacie");
            bool blad3 = testDataData(ileRazy);
            blad = (blad == true) ? true : blad3;
            Console.WriteLine("\n");

            Console.WriteLine("Sekcja druga: zapytania realizowane przez R");

            Console.WriteLine("# Wyszukiwanie urzadzen znajdujacych sie w losowym prostokacie");
            bool blad4 = testProstokat(ileRazy);
            blad = (blad == true) ? true : blad4; 

            Console.WriteLine("# Wyszukiwanie agregatow czasowych");
            bool blad5 = testAgregatyCzasowe(ileRazy);
            blad = (blad == true) ? true : blad5; 
            Console.WriteLine("\n");

            Console.WriteLine("# Wyszukiwanie agregatów powierzchniowych");
            bool blad6 = testAgregatyPowierzchniowe(ileRazy);
            blad = (blad == true) ? true : blad6;
            Console.WriteLine("\n");

            return blad;
        }

        private bool testAgregatyPowierzchniowe(int ileRazy)
        {
            bool blad = false;
            List<Rectangle> szukane = new List<Rectangle>();
            for (int i = 0; i < ileRazy; i++)
                szukane.Add(generator.generujProstokat());

            List<Decimal> resultDB = new List<Decimal>();
            List<Decimal> resultRTree = new List<Decimal>();
            List<string> Out = new List<string>();

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            List<int> ile = new List<int>();
            int cnt_1 = 0;
            for (int i = 0; i < ileRazy; i++)
            {
                Decimal x1 = szukane[i].XMin;
                Decimal y1 = szukane[i].YMin;
                Decimal x2 = szukane[i].XMax;
                Decimal y2 = szukane[i].YMax;

                ile.Add(0);

                int cnt = 0;
                resultDB.Add(0);

                List<int> ids = new List<int>();
                foreach (Urzadzenie u in ctx.Urzadzenia)
                {
                    bool a = u.Szerokosc < y1;
                    bool b = u.Szerokosc >= y2;
                    bool c = u.Dlugosc <= x1;
                    bool d = u.Dlugosc > x2;

                    if (a | b | c | d)
                        ;
                    else
                        ids.Add(u.UrzadzenieID);
                }
                Out.Add("(");
                foreach (Pomiar p in ctx.Pomiary)
                {
                    //przerobic na zapytanie
                    bool ma_wersje_przypisana = true;// p.WersjeUrzadzenia.FirstOrDefault() != null;
                    bool nalezy_do_przedzialu = ids.Contains((int)p.WersjeUrzadzenia.FirstOrDefault().UrzadzenieID);
                    bool nie_jest_stary = p.dtpomiaru > new DateTime(2024, 7, 18, 0, 0, 0);
                    if (ma_wersje_przypisana && nalezy_do_przedzialu && nie_jest_stary)
                    {
                        ile[i]++;
                        resultDB[i] += p.Wartosc;
                        Out[i] += p.Wartosc + "+";
                    }
                }
                if (ile[i] != 0)
                    resultDB[i] /= ile[i];
                else
                    resultDB[i] = 0;
                cnt_1 = cnt;
            }
            long wynik = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
            int cnt_r = 0;
            List<Decimal> ile_r = new List<Decimal>();
            for (int i = 0; i < ileRazy; i++)
            {
                (Decimal liczba_elementow, Decimal srednia) = rmvb.szukajAgregatu(szukane[i]);
                resultRTree.Add(srednia);
                ile_r.Add(liczba_elementow);//ile_r.Add(liczba_elementow);

            }
            long wynik3 = sw.ElapsedMilliseconds;

            Console.WriteLine("**********************************");
            for (int i = 0; i < ileRazy; i++)
            {
                Console.WriteLine("Szukanie agregatu powierzchniowego dla obszaru: xMin(" + szukane[i].XMin + "), " + "yMin(" + szukane[i].YMin + "), " +
                    "xMax(" + szukane[i].YMin + "), " + "yMax(" + szukane[i].YMax + "), ");
                Console.WriteLine("WARTOŚCI: Recznie: " + resultDB[i] + " vs " + "RMVB: " + resultRTree[i] + "\n");
                Console.WriteLine(Out[i] + ")/" + ile[i]);


                if (ile[i] != ile_r[i])
                {
                    if (blad == false) //powinno wykonać się tylko raz :)
                    {
                        bledy.Add("Działanie testów zakończyło się na wyszukiwaniu agregatu powierzchniowego. Poprzednie testy przebiegły pomyślnie, kolejne nie zostały zrealizowane.");
                        bledy.Add("Komunikat(y) błędu(ów): \n");
                    }
                    Console.WriteLine("Mamy rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[i] + " (baza) " +
                        ile_r[i] + " (r)");
                    //rmvb.szukajAgregatu(szukane[i]);
                    blad = true;

                    bledy.Add("Mamy rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[i] + " (baza) " +
                        ile_r[i] + " (r)");
                    bledy.Add("Współrzędne prostokąta: " + "xMin(" + szukane[i].XMin + "), " + "yMin(" + szukane[i].YMin + "), " +
                    "xMax(" + szukane[i].YMin + "), " + "yMax(" + szukane[i].YMax + ")");
                    bledy.Add("Obliczone wartości: " + "Recznie: " + resultDB[i] + " vs " + "RMVB: " + resultRTree[i] + "\n");
                }
                Console.WriteLine("**********************************");
            }
            if (!blad)
            {
                Console.WriteLine("CZASY:    Recznie: " + wynik + " vs " + "RMVB: " + wynik3);
                wyniki.Add("R | wyszukuje agregaty powierzchniowe losowego prostokata | " + wynik + " | " + wynik3);
            }

            return blad;
        }

        //wyszukuje agregat czasowy 
        private bool testAgregatyCzasowe(int ileRazy)
        {
            bool blad = false;
            //losowanie ze zwracaniem
            List<(Decimal, Decimal)> wspolrzedne = new List<(Decimal, Decimal)>();
            for (int i = 0; i < ileRazy; i++)
                wspolrzedne.Add(generator.wylosujWspolrzedne());

            List<Decimal> wynikBD = new List<Decimal>();
            List<Decimal> wynikR = new List<Decimal>();

            Stopwatch sw;
            int cnt_1 = 0;

            sw = Stopwatch.StartNew();
            List<int> liczby = new List<int>();
            List<int> id = new List<int>();
            for (int i = 0; i < ileRazy; i++)
            {
                (Decimal x, Decimal y) = wspolrzedne[i];
                wynikBD.Add(0);
                liczby.Add(0);
                id.Add(-1);
                id[i] = ctx.Urzadzenia
                    .AsNoTracking()
                    .Where(u => u.Szerokosc == y)
                    .Where(u => u.Dlugosc == x)
                    .First()
                    .UrzadzenieID;

                if (id[i] != -1)
                {
                    int aktualne_id = id[i];
                    List<Pomiar> pomiary = ctx.Pomiary
                                            .AsNoTracking()
                                            .Where(p => p.WersjeUrzadzenia.FirstOrDefault().UrzadzenieID == aktualne_id)
                                            .Where(p => p.dtpomiaru > new DateTime(2024, 7, 18, 0, 0, 0)) //zparametryzowac
                                            .ToList();
                    liczby[i] += pomiary.Count;
                    foreach (Pomiar p in pomiary) wynikBD[i] += p.Wartosc;

                    if (liczby[i] != 0)
                        wynikBD[i] /= liczby[i];
                    else
                        wynikBD[i] = 0;
                }
                else
                {
                    Console.WriteLine("Urzadzenie o wsp. " + x + " " + y + " nie istnieje w bazie");
                    blad = true;
                }
            }
            long czasBD = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();

            List<Urzadzenie> resDevices = new List<Urzadzenie>();
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                (Decimal x, Decimal y) = wspolrzedne[i];
                wynikR.Add(rmvb.szukajAgregatuCzasowego(x, y));
            }
            long czas = sw.ElapsedMilliseconds;
            Console.WriteLine("**********************************");
            for (int i = 0; i < ileRazy; i++)
            {
                (Decimal x, Decimal y) = wspolrzedne[i];
                Console.WriteLine("Szukanie agregatu czasowego dla urządzenia o (x, y) = (" + x + ", " + y + ") i id = " + id[i].ToString());
                Console.WriteLine("WARTOŚCI: Baza: " + wynikBD[i] + " vs " + "Rtree: " + wynikR[i]);
                if (wynikBD[i] != wynikR[i] || repo.pobierzUrzadzenia()[id[i]].get_liczba_suma().Item1 != liczby[i])
                {
                    if (!blad) {
                        bledy.Add("Działanie testów zakończyło się na wyszukiwaniu agregatu czasowego. Poprzednie testy przebiegły pomyślnie, kolejne nie zostały zrealizowane.");
                        bledy.Add("Komunikat(y) błędu(ów): \n");
                    }
                    blad = true;

                    if (wynikBD[i] != wynikR[i])
                    {
                        bledy.Add("Mamy rozbieznosc miedzy obliczonymi wartościami: " + wynikR[i] + "(R) " + wynikBD[i] + "(ręcznie)");
                        Console.WriteLine("Mamy rozbieznosc miedzy obliczonymi wartościami.");
                    }

                    if (repo.pobierzUrzadzenia()[id[i]].get_liczba_suma().Item1 != liczby[i])
                    {
                        bledy.Add("Mamy rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + liczby[i] + " (baza) " +
                            repo.pobierzUrzadzenia()[id[i]].get_liczba_suma().Item1 + " (r)");

                        Console.WriteLine("Mamy rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu czasowego urządzenia o współrzędnych: (" +
                            wspolrzedne[i].Item1 + "," + wspolrzedne[i].Item2 + ") i id: " + id[i]);
                        Console.WriteLine("Na podstawie " + repo.pobierzUrzadzenia()[id[i]].get_liczba_suma().Item1 + " (R) " + liczby[i] + " (ręcznie)" + " pomiarów");
                    }
                    bledy.Add("");

                }
                Console.WriteLine("**********************************");
            }
            Console.WriteLine("CZASY: Baza: " + czasBD + " vs " + "Rtree: " + czas);
            wyniki.Add("R | wyszukuje agregat czasowy losowego urządzenia | " + czasBD + " | " + czas);
            return blad;

        }

        //wyszukiwanie urzadzen znajdujacych sie w losowym prostokacie x ileRazy

        private bool testProstokat(int ileRazy) {
            bool blad = false;
            List<Rectangle> searchRect = new List<Rectangle>();
            for(int i = 0; i < ileRazy; i++)
                searchRect.Add(generator.generujProstokat()); 

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            List<List<Urzadzenie>> cnt_1 = new List<List<Urzadzenie>>();
            for (int i = 0; i < ileRazy; i++)
            {
                Rectangle rect = searchRect[i];
                cnt_1.Add(ctx.Urzadzenia
                .AsNoTracking()
                //contain zawiera intrsects, wyszukujemy punkty, a nie tylko porstokaty
                /* //intersects
                 .Where(u => rect.XMin < u.Dlugosc)
                 .Where(u => rect.XMax > u.Dlugosc)
                 .Where(u => rect.YMin < u.Szerokosc)
                 .Where(u => rect.YMax > u.Szerokosc)
                 */
                 //contains
                .Where(u => rect.XMin <= u.Dlugosc)
                .Where(u => rect.YMin <= u.Szerokosc)
                .Where(u => rect.XMax >= u.Dlugosc)
                .Where(u => rect.YMax >= u.Szerokosc)
                .ToList());
            }
            long wynik = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
                

            List<List<Urzadzenie>> cnt_r = new List<List<Urzadzenie>>();
            for (int i = 0; i < ileRazy; i++)
            {
                cnt_r.Add(rmvb.szukaj(searchRect[i]));
            }
            long wynik3 = sw.ElapsedMilliseconds;

            Console.WriteLine("**********************************");
            for (int i = 0; i < ileRazy; i++)
            {
                Console.WriteLine("Prostokat: " + searchRect[i].XMin + " " + searchRect[i].XMax + "(x) " + searchRect[i].YMin + " " + searchRect[i].YMax + "(y)");
                Console.WriteLine("Znaleziono " + cnt_r[i].Count.ToString() + "(rt) " + cnt_1[i].Count.ToString() + "(zapytanie w bazie)");
                if (cnt_r[i].Count != cnt_1[i].Count)
                {
                    blad = true;
                    List<Urzadzenie> nadmiarowe = new List<Urzadzenie>();
                    if (cnt_r[i].Count > cnt_1[i].Count)
                    {
                        Console.WriteLine("R-drzewo dodatkowo znalazło następujące urządzenia: ");
                        nadmiarowe = (cnt_r[i].Where(u => !cnt_1[i].Any(u1 => (u1.UrzadzenieID == u.UrzadzenieID))).ToList());
                    }
                    else
                    {
                        Console.WriteLine("Baza dodatkowo znalazła następujące urządzenia: ");
                        nadmiarowe = (cnt_1[i].Where(u => !cnt_r[i].Any(u1 => (u1.UrzadzenieID == u.UrzadzenieID))).ToList());
                    }

                    foreach (Urzadzenie u in nadmiarowe)
                    {
                        Console.WriteLine("UrzadzenieID: " + u.UrzadzenieID + " x: " + u.Dlugosc + " y: " + u.Szerokosc);
                    }

                    rmvb.szukaj(searchRect[i]);

                }
                Console.WriteLine("**********************************");
            }

            Console.WriteLine("RMVB: " + wynik3 + " vs " + "Recznie: " + wynik);
            wyniki.Add("R | wyszukuje urzadzenia znajdujace sie w losowym prostokacie |" + wynik +  " | " + wynik3);
            return blad;
        }


        //wyszukuje urządzenie w podanym punkcie x ileRazy
        private void testPunkt(int ileRazy) { 
            throw new NotImplementedException();
        }

        //wyszukiwanie losowego urządzenia po dacie i id x ileRazy
        public bool testDataId(int ileRazy)
        {

            bool blad = false;
            List<Wersja> szukane_wersje = generator.wylosujWersje(ileRazy);
            
            List<Wersja> odnalezione_baza = new List<Wersja>();
            List<Wersja> odnalezione_rmvb = new List<Wersja>();

            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++) {
                int id = szukane_wersje[i].UrzadzenieID;
                DateTime dt = szukane_wersje[i].dataOstatniejModyfikacji;
                Wersja szukana = ctx.Wersje
                    .AsNoTracking()
                    .Where(u => u.dataOstatniejModyfikacji <= dt)
                    .Where(u => u.dataWygasniecia > dt)
                    .Where(u => u.UrzadzenieID == id)
                    .FirstOrDefault(); //czasami nie dziala:/
                if (szukana == null)
                {
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
                    blad = true;
                }
                else { 
                    odnalezione_baza.Add(szukana);
                }
            }
            long czas_baza = sw.ElapsedMilliseconds;
            if (!blad)
            {
                Console.WriteLine("Baza w czasie: " + czas_baza + " ms.");
            }

            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                int id = szukane_wersje[i].UrzadzenieID;
                DateTime dt =  szukane_wersje[i].dataOstatniejModyfikacji;
                Wersja szukana = rmvb.szukaj(id, dt);
                if (szukana == null)
                {
                    Console.WriteLine("Uwaga: RMVB nie odnalazlo rekordu.");
                    rmvb.szukaj(id, dt);
                    blad = true;
                }
                else { 
                    odnalezione_rmvb.Add(szukana);
                }
            }
            long czas_mvb = sw.ElapsedMilliseconds;
            if (!blad)
            {
                Console.WriteLine("MVB w czasie: " + czas_mvb + "ms.");
                wyniki.Add("MVB | wyszukiwanie losowego urządzenia po dacie i id | " + czas_baza + " | " + czas_mvb );
            }
            else
            {
                //except nie zadziala
                int index_baza = 0;
                int index_rmvb = 0;
                for (int index = 0; index < szukane_wersje.Count; index++)
                {
                    Wersja wersja = szukane_wersje[index];
                    int id_urzadzenia = wersja.UrzadzenieID;
                    int id_wersji = wersja.WersjaID;

                    bool odnaleziono_baza = false;
                    bool odnaleziono_rmvb = false;

                    if (odnalezione_baza[index_baza].UrzadzenieID == id_urzadzenia &&
                        odnalezione_baza[index_baza].WersjaID == id_wersji)
                    {
                        odnaleziono_baza = true;
                        index_baza++;
                    }
                    else
                    {
                        //nieodnaleziono
                    }

                    if (odnalezione_rmvb[index_rmvb].UrzadzenieID == id_urzadzenia &&
                        odnalezione_baza[index_rmvb].WersjaID == id_wersji)
                    {
                        odnaleziono_rmvb = true;
                        index_rmvb++;
                    }
                    else
                    {
                        //nieodnaleziono
                    }

                    Console.WriteLine("id: " + id_urzadzenia + " ver: " + id_wersji
                        + " baza: " + odnaleziono_baza.ToString() + " rmvb: " + odnaleziono_rmvb.ToString());
                }

            }
            return blad;
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
        public bool testId(int ileRazy) {
            bool blad = false;
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
                {
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu o id " + id + ".");
                    blad = true;
                }
            }
            long czas_baza = sw.ElapsedMilliseconds;
            if (!blad)
            {
                Console.WriteLine("Baza w czasie: " + czas_baza + " ms.");
            }

            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                szukana = rmvb.szukaj(szukane_id[i]);
                if (szukana == null)
                {
                    Console.WriteLine("Uwaga: RMVB nie odnalazlo rekordu.");
                    blad = true;
                }
            }

            long czas_mvb = sw.ElapsedMilliseconds;
            if (!blad)
            {
                Console.WriteLine("MVB w czasie: " + czas_mvb + " ms.");
                wyniki.Add("MVB | wyszukiwanie ostatniej wersji po id | " + czas_baza + " | " + czas_mvb );
            }
            return blad;
        }

        //wyszukiwanie po id i wersji
        public bool testIdV(int ileRazy) {
            bool blad = false;

            //doprowadzic spowrotem do porzadku (losowe)
            List<(int, int)> szukane_id_v = new List<(int, int)>();
            for (int i = 0; i < ileRazy; i++)
            {
                int losowe_urzadzenie_id = repo.pobierzUrzadzenia().ElementAt(rnd.Next(repo.pobierzUrzadzenia().Count - 1)).Value.UrzadzenieID;
                int losowa_wersja_id = repo.pobierzUrzadzeniaWersje().ElementAt(losowe_urzadzenie_id).Value.ElementAt(rnd.Next(repo.pobierzUrzadzeniaWersje().ElementAt(losowe_urzadzenie_id).Value.Count - 1)).WersjaID;
                szukane_id_v.Add((losowe_urzadzenie_id, losowa_wersja_id));
            }
            
            //>
            List<Wersja?> znalezione_baza = new List<Wersja>();
            sw = Stopwatch.StartNew();
            for (int i = 0; i < szukane_id_v.Count(); i++)
            {
                znalezione_baza.Add(null);
                int id = szukane_id_v[i].Item1;
                int v = szukane_id_v[i].Item2;

                znalezione_baza[i] = ctx.Wersje
                .AsNoTracking()
                .FirstOrDefault(u => u.UrzadzenieID == id && u.WersjaID == v);

                if (znalezione_baza[i] == null)
                {
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
                    blad = true;
                }
            }

            long czas_baza = sw.ElapsedMilliseconds;
                

            sw = Stopwatch.StartNew();
            List<Wersja?> znalezione_rmvb =  new List<Wersja?>();
            for (int i = 0; i < szukane_id_v.Count(); i++)
            {
                znalezione_rmvb.Add(null);
                int id = szukane_id_v[i].Item1;
                int v = szukane_id_v[i].Item2;
                znalezione_rmvb[i] = rmvb.szukaj(id, v);
                if (znalezione_rmvb[i] == null)
                {
                    Console.WriteLine("Uwaga: RMVB nie odnalazlo rekordu.");
                    //do debuggowania
                    znalezione_rmvb[i] = rmvb.szukaj(id, v);
                    blad = true;
                }
            }
            long czas_mvb = sw.ElapsedMilliseconds;
            if (!blad)
            {
                Console.WriteLine("CZAS WYKONANIA: baza: " + czas_baza + " rmvb: " + czas_mvb);
                wyniki.Add("MVB | wyszukiwanie losowych urządzeń po id i wersji | " + czas_baza + " | " + czas_mvb);
            }
            else {
                for (int i = 0; i < ileRazy; i++)
                {
                    if (znalezione_baza[i] == null && znalezione_rmvb[i] == null)
                    {
                        Console.WriteLine("Nie odnaleziono urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                    }
                    else if (znalezione_baza[i] == null) {
                        Console.WriteLine("Baza nie odnalazła urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                    }
                    if (znalezione_rmvb[i] == null) {
                        Console.WriteLine("RMVB nie odnalazło urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                    }
                }
            }
            return blad;
        }

        public List<(DateTime, DateTime)> wylosujPrzedzialy(DateTime poczatek, DateTime koniec, int liczba_przedzialow) {
            List<(DateTime, DateTime)> wyjsciowa = new List<(DateTime, DateTime)>();
            long range = (koniec - poczatek).Ticks; //ile czasu miedzy poczatkiem a koncem

            for (int i = 0; i < liczba_przedzialow; i++) {
                DateTime losowa1 = poczatek.AddTicks((long)(rnd.NextDouble() * range));
                DateTime losowa2 = poczatek.AddTicks((long)(rnd.NextDouble() * range));

                if (losowa1 > losowa2)
                    wyjsciowa.Add((losowa2, losowa1));
                else
                    wyjsciowa.Add((losowa1, losowa2));
            }
            return wyjsciowa;
        }

        //wyszukiwanie wersji urządzeń aktywnych w losowym oknie czasowym
        public bool testDataData(int ileRazy) {
            bool blad = false;            
            //najwczesniejsza data poczatku
            DateTime poczatek = ctx.Wersje.OrderBy(u=>u.dataOstatniejModyfikacji).FirstOrDefault().dataOstatniejModyfikacji;
            //najpozniejsza data konca, wyszukujemy tylko z martwych urzadzen
            DateTime koniec_nie_9999 = ctx.Wersje.Where(u=>u.dataWygasniecia != DateTime.MaxValue).OrderByDescending(u => u.dataWygasniecia).Select(u => u.dataWygasniecia).First();

            List<(DateTime, DateTime)> losowe_przedzialy = wylosujPrzedzialy(poczatek, koniec_nie_9999, ileRazy);
            

            var szukane_wersje = new List<Wersja>();
            var szukane_wersje_mvb = new List<Wersja>();

            //Console.WriteLine(poczatek.Ticks + "-" + koniec.Ticks);
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                DateTime start = losowe_przedzialy[i].Item1;
                DateTime end = losowe_przedzialy[i].Item2;
                szukane_wersje.AddRange(ctx.Wersje.AsNoTracking().Where(u => u.dataOstatniejModyfikacji >= start).Where(u => u.dataWygasniecia < end).ToList());
            }
            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza: " + szukane_wersje.Count + " w czasie: " + czas_baza + " ms.");
            
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                DateTime start = losowe_przedzialy[i].Item1;
                DateTime end = losowe_przedzialy[i].Item2; 
                szukane_wersje_mvb.AddRange(rmvb.szukaj(start, end));
            }
            long czas_mvb = sw.ElapsedMilliseconds;

            if (szukane_wersje.Count != szukane_wersje_mvb.Count)
            {
                /*                var duplicates = szukane_wersje_mvb
                                .GroupBy(i => i)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key).ToList();*/
                //except nie zadziala
                var nieznalezione = szukane_wersje
                                    .Where(d => !szukane_wersje_mvb.Any(mvb =>
                                        mvb.UrzadzenieID == d.UrzadzenieID &&
                                        mvb.WersjaID == d.WersjaID))
                                    .ToList();
                if (nieznalezione.Count != 0)
                    Console.WriteLine("Nie znaleziono następujących urządzeń: ");
                foreach (var u in nieznalezione)
                    Console.WriteLine(u.UrzadzenieID + "v" + u.WersjaID + " " + u.dataOstatniejModyfikacji.Ticks + "-" + u.dataWygasniecia.Ticks);

                List<Wersja> nadmiarowe = new List<Wersja>(szukane_wersje_mvb);
                foreach (var elem in szukane_wersje_mvb.Distinct())
                    nadmiarowe.Remove(elem);

                if (szukane_wersje_mvb.Distinct().Count() != szukane_wersje_mvb.Count())
                    Console.WriteLine("Znaleziono nadmiarowe urządzenia: ");
                foreach (var u in nadmiarowe)
                    Console.WriteLine(u.UrzadzenieID + "v" + u.WersjaID + " " + u.dataOstatniejModyfikacji.Ticks + "-" + u.dataWygasniecia.Ticks);
                blad = true;
            }
            else {
                Console.WriteLine("RMVB: " + szukane_wersje_mvb.Count + " w czasie: " + czas_mvb + " ms.");
                wyniki.Add("MVB | wyszukiwanie wersji urządzeń aktywnych w losowym oknie czasowym | " + czas_baza + " | " + czas_mvb );
            }
            return blad;
        }

        internal void zapiszWyniki(string v)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(v, "Testy.txt")))
            {
                foreach (string linijka in wyniki)
                    outputFile.WriteLine(linijka);
            }
        }

        internal void zapiszBledy(string v)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(v, "Bledy.txt")))
            {
                foreach (string linijka in bledy)
                    outputFile.WriteLine(linijka);
            }
        }
    }
}
