using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RMVB_konsola.MVB;

namespace RMVB_konsola
{
    //singleton?
    internal class Test
    {
        public static Repo repo;
        public static Kontekst ctx;
        internal Stopwatch sw;
        public static Drzewo mvb;

        //konstruktor z polami statycznymi

        //wyszukiwanie losowego urządzenia po dacie i id x ileRazy
        public void testDataId(int ileRazy)
        {
            Random rnd = new Random();
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
    }
}
