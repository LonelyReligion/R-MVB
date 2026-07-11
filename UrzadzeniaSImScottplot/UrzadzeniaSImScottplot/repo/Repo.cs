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
       
        public Dictionary<int, Urzadzenie> urzadzenia = new Dictionary<int, Urzadzenie>();

        //list, bo często sięgamy do ostatniego (największego) elementu
        private Dictionary<int, List<int>> urzadzenia_wersje = new Dictionary<int, List<int>>();

        internal Dictionary<int, List<int>> zwroc_urzadzenie_wersje() { return urzadzenia_wersje; }
        
        public void saveDevice(Urzadzenie device)
        {
            using (var ctx = new Kontekst())
            {
                ctx.Urzadzenia.Add(device);
                urzadzenia_wersje.Add(device.UrzadzenieID, new List<int>());
                ctx.SaveChanges();
            }

            urzadzenia.Add(device.UrzadzenieID, device);
        }

        public void saveMeasurement(int UrzadzenieID, Pomiar p, Wersja alfa) {
            using (var ctx = new Kontekst())
            {
                ctx.Wersje.Attach(alfa);
                ctx.Entry(alfa).Collection(x => x.Pomiary).Load();

                alfa.Pomiary.Add(p);
                ctx.Pomiary.Add(p);
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
            urzadzenia = new Dictionary<int, Urzadzenie>();
        }

        public void InicjujBazeDanych() {
            using (var ctx = new Kontekst())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM Urzadzenies");
                ctx.Database.ExecuteSqlCommand("DELETE FROM Wersjas");
                ctx.Database.ExecuteSqlCommand("DELETE FROM Pomiars");
                ctx.Database.ExecuteSqlCommand("DELETE FROM TimeAggregates");

                ctx.Urzadzenia.FirstOrDefault(); //ma przyspieszyc pierwsze zapytanie
            }
        }
        public void saveTimeAggregate(TimeAggregate timeAggregate)
        {
            using (var ctx = new Kontekst())
            {
                //brzydkie rozwiazanie dzieki niemu nie ma bledu
                foreach (var entry in ctx.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }

                ctx.TimeAggregates.Add(timeAggregate);
                ctx.SaveChanges();
            }
        }

        public void saveSpaceAggregate(SpaceAggregate spaceAggregate)
        {
            using (var ctx = new Kontekst())
            {
                ctx.SpaceAggregates.Add(spaceAggregate);
                ctx.SaveChanges();
            }
        }
    }
}
