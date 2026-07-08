using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrzadzeniaSImScottplot
{
    public class Repo
    {

        public Repo() { }
        //zmienic na przechowywanie samych id urzadzen i wersji
        //list, bo często sięgamy do ostatniego (największego) elementu
        private Dictionary<int, List<int>> urzadzenia_wersje = new Dictionary<int, List<int>>();



        internal Dictionary<int, List<int>> zwroc_urzadzenie_wersje() { return urzadzenia_wersje; }
        //override jest konieczne inaczej realizowana jest wersja z klasy bazowej
        //"Modyfikator override jest wymagany do rozszerzenia lub zmodyfikowania abstrakcyjnej lub wirtualnej implementacji dziedziczonej metody, właściwości, indeksatora lub zdarzenia."
        public void saveDevice(Urzadzenie device)
        {
            using (var ctx = new Kontekst())
            {
                ctx.Urzadzenia.Add(device);
                urzadzenia_wersje.Add(device.UrzadzenieID, new List<int>());
                ctx.SaveChanges();
            }
        }

        public void saveVersion(Wersja v)
        {
   
            using (var ctx = new Kontekst())
            {

                //TAK MUSI BYC BO WSPOLDZIELIMY POMIARY MIEDZY WERSJAMI (I WERSJE MIĘDZY POMIARAMI?)
                foreach (var p in v.Pomiary)
                {
                    ctx.Entry(p).State = EntityState.Unchanged;
                }


                ctx.Entry(v).State = EntityState.Added;

                Debug.WriteLine(ctx.ChangeTracker.Entries<Wersja>().Count());
                foreach (var e in ctx.ChangeTracker.Entries<Wersja>())
                {
                    Debug.WriteLine($"{e.Entity.UrzadzenieID} {e.Entity.WersjaID} {e.State}");
                }

                ctx.SaveChanges();
            }

            urzadzenia_wersje[v.UrzadzenieID].Add(v.WersjaID);

        }

        public bool czyUrzadzenieIstnieje(int UrzadzenieID)
        {
            using (var ctx = new Kontekst())
            {
                return ctx.Urzadzenia.Where(u => u.UrzadzenieID == UrzadzenieID) != null;
            }
        }

        public bool czyWersjaIstnieje(int UrzadzenieID, int WersjaID)
        {
            if (!czyUrzadzenieIstnieje(UrzadzenieID))
                return false;
            else
                foreach (int id in urzadzenia_wersje[UrzadzenieID])
                    if (id == WersjaID)
                        return true;
            return false;
        }



        public void Reset()
        {
            urzadzenia_wersje = new Dictionary<int, List<int>>();
        }

        public void InicjujBazeDanych() {
            using (var ctx = new Kontekst())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM Urzadzenies");
                ctx.Database.ExecuteSqlCommand("DELETE FROM Wersjas");
                ctx.Database.ExecuteSqlCommand("DELETE FROM Pomiars");

                ctx.Urzadzenia.FirstOrDefault(); //ma przyspieszyc pierwsze zapytanie
            }
        }
    }
}
