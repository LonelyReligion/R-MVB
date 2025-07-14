﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RMVB_konsola.MVB;
using System.Data;
using System.Collections;

namespace RMVB_konsola
{
    //singleton?
    internal class Test
    {
        public static Repo repo;
        public static Kontekst ctx;
        public static Drzewo mvb;

        internal Stopwatch sw;
        internal Random rnd = new Random();

        //konstruktor z polami statycznymi

        //wyszukiwanie losowego urządzenia po dacie i id x ileRazy
        public void testDataId(int ileRazy)
        {
            List<Urzadzenie> szukane_urzadzenia = new List<Urzadzenie>();
            for (int i = 0; i < ileRazy; i++) {
                Urzadzenie losowe_urzadzenie = repo.urzadzenia.ElementAt(rnd.Next(repo.urzadzenia.Count - 1));
                szukane_urzadzenia.Add(losowe_urzadzenie);
            }

            Urzadzenie? szukane = null;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++) {
                int id = szukane_urzadzenia[i].UrzadzenieID;
                DateTime dt = szukane_urzadzenia[i].dataOstatniejModyfikacji;
                szukane = ctx.Urzadzenia
                    .AsNoTracking()
                    .Where(u => u.dataOstatniejModyfikacji <= dt)
                    .Where(u => u.dataWygasniecia > dt)
                    .Where(u => u.UrzadzenieID == id)
                    .FirstOrDefault(); //czasami nie dziala:/
                if (szukane == null)
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
            }
            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza w czasie: " + czas_baza + " ms.");

            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                int id = szukane_urzadzenia[i].UrzadzenieID;
                DateTime dt =  szukane_urzadzenia[i].dataOstatniejModyfikacji;
                szukane = mvb.szukaj(id, dt);
            }
            long czas_mvb = sw.ElapsedMilliseconds;
            Console.WriteLine("MVB w czasie: " + czas_mvb + "ms.");
        }

        //wyszukiwanie ostatniej wersji po id
        public void testId(int ileRazy) {
            List<int> szukane_id = new List<int>();
            for (int i = 0; i < ileRazy; i++)
            {
                int losowe_urzadzenie = repo.urzadzenia.ElementAt(rnd.Next(repo.urzadzenia.Count - 1)).UrzadzenieID;
                szukane_id.Add(losowe_urzadzenie);
            }

            Urzadzenie? szukane = null;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++) {
                int id = szukane_id[i];
                szukane = ctx.Urzadzenia
                    .AsNoTracking() //nie uzywamy zbuforowanych (wynikow poprzednich wykonan)
                    .Where(u => u.UrzadzenieID == id)
                    .OrderByDescending(u => u.Wersja)
                    .FirstOrDefault();
                if (szukane == null)
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
            }
            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza w czasie: " + czas_baza + " ms.");
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
                szukane = mvb.szukaj(szukane_id[i]);
            long czas_mvb = sw.ElapsedMilliseconds;
            Console.WriteLine("MVB w czasie: " + czas_mvb + " ms.");
        }

        //wyszukiwanie po id i wersji
        public void testIdV(int ileRazy) {
            List<(int, int)> szukane_id_v = new List<(int, int)>();
            for (int i = 0; i < ileRazy; i++)
            {
                Urzadzenie losowe_urzadzenie = repo.urzadzenia.ElementAt(rnd.Next(repo.urzadzenia.Count - 1));
                szukane_id_v.Add((losowe_urzadzenie.UrzadzenieID, losowe_urzadzenie.Wersja));
            }

            Urzadzenie? szukane = null;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                int id = szukane_id_v[i].Item1;
                int v = szukane_id_v[i].Item2;

                szukane = ctx.Urzadzenia
                .AsNoTracking()
                .FirstOrDefault(u => u.UrzadzenieID == id && u.Wersja == v);

                if (szukane == null)
                    Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
            }

            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza: " + szukane.UrzadzenieID + "v" + szukane.Wersja + " w czasie: " + czas_baza + " ms.");

            sw = Stopwatch.StartNew();
            for (int i = 0; i < ileRazy; i++)
            {
                int id = szukane_id_v[i].Item1;
                int v = szukane_id_v[i].Item2;
                szukane = mvb.szukaj(id, v);
            }
            long czas_mvb = sw.ElapsedMilliseconds;
            Console.WriteLine("MVB: " + szukane.UrzadzenieID + "v" + szukane.Wersja + " w czasie: " + czas_mvb + " ms.");
        }

        public void testDataData(int ileRazy) {
            DateTime poczatek = ctx.Urzadzenia.OrderBy(u=>u.dataOstatniejModyfikacji).FirstOrDefault().dataOstatniejModyfikacji.AddTicks(-10);
            DateTime koniec = ctx.Urzadzenia
                                .OrderByDescending(u => u.dataWygasniecia)
                                .Select(u => u.dataWygasniecia)
                                .First();
            int range = ((TimeSpan)(koniec - poczatek)).Milliseconds;
            List<DateTime> randos = new List<DateTime>();
            randos.Add(poczatek.AddTicks(rnd.Next(range)));

            var szukane_urzadzenia = new List<Urzadzenie>();
            var szukane_urzadzenia_mvb = new List<Urzadzenie>();

            //Console.WriteLine(poczatek.Ticks + "-" + koniec.Ticks);
            sw = Stopwatch.StartNew();
            for(int i = 0; i < ileRazy; i++)
                szukane_urzadzenia = ctx.Urzadzenia.AsNoTracking().Where(u => u.dataOstatniejModyfikacji >= poczatek).Where(u => u.dataWygasniecia < koniec).ToList();
            long czas_baza = sw.ElapsedMilliseconds;
            Console.WriteLine("Baza: " + szukane_urzadzenia.Count + " w czasie: " + czas_baza + " ms.");
            
            sw = Stopwatch.StartNew();
            szukane_urzadzenia_mvb = mvb.szukaj(poczatek, koniec);
            long czas_mvb = sw.ElapsedMilliseconds;
            Console.WriteLine("MVB: " + szukane_urzadzenia_mvb.Count + " w czasie: " + czas_mvb + " ms.");

            if (szukane_urzadzenia.Count != szukane_urzadzenia_mvb.Count)
            {
                //except nie zadziala
                var nieznalezione = szukane_urzadzenia
                                    .Where(d => !szukane_urzadzenia_mvb.Any(mvb =>
                                        mvb.UrzadzenieID == d.UrzadzenieID &&
                                        mvb.Wersja == d.Wersja))
                                    .ToList();
                foreach(var u in nieznalezione)
                    Console.WriteLine(u.UrzadzenieID + "v" + u.Wersja + " " + u.dataOstatniejModyfikacji.Ticks + "-" + u.dataWygasniecia.Ticks);
            }
        }
    }
}
