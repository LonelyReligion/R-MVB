using System.Data.Entity;
using UrzadzeniaSim.Model.RMVB.R;

namespace UrzadzeniaSim.Model.DB
{
    public class InDBStorage : TreeRepository
    {
        public static Kontekst ctx;

        //nie abstract, bo ma ciało
        //virtual jest konieczne do tego, aby klasa Repo mogła ją napisać
        //"znacza to, że dana metoda może zostać nadpisana w klasie która dziedziczy po klasie w której jest ta metoda zdefiniowana"
        public virtual void saveDevice(Urzadzenie_Model device)
        {
            ctx.SaveChanges();
        }

        public virtual void saveVersion(Wersja version) {
            ctx.SaveChanges();
        }

        public void saveMeasurement(Pomiar measure)
        {
            ctx.Pomiary.Add(measure);
            ctx.SaveChanges();
        }

        public void saveTimeAggregate(TimeAggregate timeAggregate)
        {
            //brzydkie rozwiazanie dzieki niemu nie ma bledu
            foreach (var entry in ctx.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }

            ctx.TimeAggregates.Add(timeAggregate);
            ctx.SaveChanges();
        }

        public void saveSpaceAggregate(SpaceAggregate spaceAggregate)
        {
            ctx.SpaceAggregates.Add(spaceAggregate);
            ctx.SaveChanges();
        }


        public void saveSrednia(Srednia srednia)
        {
            ctx.Srednie.Add(srednia);
            ctx.SaveChanges();
        }

        public void Reset() {
            ctx.Database.ExecuteSqlCommand("delete from Urzadzenie_Model");
        }
    }
}
