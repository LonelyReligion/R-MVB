using System.Data.Entity;

namespace UrzadzeniaSImScottplot.repo
{
    public class Repo
    {

        public Repo() { }
       
        public Dictionary<int, Urzadzenie> urzadzenia = new Dictionary<int, Urzadzenie>();

        //list, bo często sięgamy do ostatniego (największego) elementu
        private Dictionary<int, List<int>> urzadzenia_wersje = new Dictionary<int, List<int>>();

        internal Dictionary<int, List<int>> zwroc_urzadzenie_wersje() { return urzadzenia_wersje; }
        
        //do zwrocenia wszystkich
        private List<Wersja> wersje = new List<Wersja>();

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
                ctx.Entry(alfa).State = EntityState.Modified;

                ctx.Pomiary.Add(p);
                ctx.SaveChanges();
            }
        }
        public Dictionary<int, Urzadzenie> pobierzUrzadzenia()
        {
            return urzadzenia;
        }


        public void saveVersion(Wersja v)
        {

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

            urzadzenia_wersje[v.UrzadzenieID].Add(v.WersjaID);
            this.pobierzUrzadzenia()[v.UrzadzenieID].Wersje.Add(v);
        }

        //uzywane tylko do dezaktywowania
        public void modifyVersion(Wersja v)
        {

            using (var ctx = new Kontekst())
            {
                foreach (var p in v.Pomiary)
                {
                    ctx.Entry(p).State = EntityState.Unchanged;
                }

                ctx.Entry(v).State = EntityState.Modified;


                for (int i = 0; i < wersje.Count(); i++)
                {
                    Wersja obecnie_rozeznawana = wersje[wersje.Count() - 1 - i];
                    if (obecnie_rozeznawana.UrzadzenieID == v.UrzadzenieID && obecnie_rozeznawana.WersjaID == v.WersjaID)
                        wersje[wersje.Count() - 1 - i] = v;
                }
                
                ctx.SaveChanges();
            }

            urzadzenia_wersje[v.UrzadzenieID].Add(v.WersjaID);
            this.pobierzUrzadzenia()[v.UrzadzenieID].Wersje.Add(v);
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
                ctx.Database.ExecuteSqlCommand("DELETE FROM Pomiars");
                ctx.Database.ExecuteSqlCommand("DELETE FROM Wersjas");
                ctx.Database.ExecuteSqlCommand("DELETE FROM TimeAggregates");
                ctx.Database.ExecuteSqlCommand("DELETE FROM Urzadzenies");
                ctx.Urzadzenia.FirstOrDefault(); //ma przyspieszyc pierwsze zapytanie
            }
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

        public List<Wersja> pobierzWersje()
        {
            return wersje;
        }
    }
}
