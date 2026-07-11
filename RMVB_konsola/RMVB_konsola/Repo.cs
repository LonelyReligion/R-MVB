using RMVB_konsola.R;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Repo
    {

        //zmienic na przechowywanie samych id urzadzen i wersji
        //list, bo często sięgamy do ostatniego (największego) elementu
        private Dictionary<int, List<Wersja>> urzadzenia_wersje = new Dictionary<int, List<Wersja>>();
        private Dictionary<int, Urzadzenie> urzadzenia = new Dictionary<int, Urzadzenie>();
        //do zwrocenia wszystkich
        private List<Wersja> wersje = new List<Wersja>();

        internal Dictionary<int, List<Wersja>>  zwroc_urzadzenie_wersje() { return urzadzenia_wersje; }

        public void saveDevice(Urzadzenie device) {

            using (var ctx = new Kontekst())
            {
                ctx.Urzadzenia.Add(device);
                urzadzenia_wersje.Add(device.UrzadzenieID, new List<Wersja>());
                urzadzenia.Add(device.UrzadzenieID, device);
                ctx.SaveChanges();
            }
        }

        public void saveVersion(Wersja v) {

            using (var ctx = new Kontekst())
            {
                foreach (var p in v.Pomiary)
                {
                    ctx.Entry(p).State = EntityState.Unchanged;
                }

                ctx.Entry(v).State = EntityState.Added;


                wersje.Add(v);
                ctx.SaveChanges();
            }

            urzadzenia_wersje[v.UrzadzenieID].Add(v);
            this.pobierzUrzadzenia()[v.UrzadzenieID].Wersje.Add(v);
        }

        public bool czyUrzadzenieIstnieje(int UrzadzenieID) {
            return urzadzenia.ContainsKey(UrzadzenieID);
        }

        public bool czyWersjaIstnieje(int UrzadzenieID, int WersjaID) {
            if (!urzadzenia.ContainsKey(UrzadzenieID))
                return false;
            else 
                foreach (Wersja w in urzadzenia_wersje[UrzadzenieID]) 
                    if(w.WersjaID == WersjaID)
                        return  true;
            return false;
        }

        public Dictionary<int, Urzadzenie> pobierzUrzadzenia() {
            return urzadzenia;
        }

        public List<Wersja> pobierzWersje() {
            return wersje;
        }

        public void Reset() {
            urzadzenia_wersje = new Dictionary<int, List<Wersja>>();
            urzadzenia = new Dictionary<int, Urzadzenie>();
            wersje = new List<Wersja>();
        }

        public void saveTimeAggregate(TimeAggregate timeAggregate)
        {
            using (var ctx = new Kontekst())
            {
                //brzydkie rozwiazanie dzieki niemu nie ma bledu
                /*                foreach (var entry in ctx.ChangeTracker.Entries())
                                {
                                    entry.State = EntityState.Detached;
                                }*/

                var u = ctx.Urzadzenia.Find(timeAggregate.DeviceId);
                u.rTimeAggregate = timeAggregate.tAValue;

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
