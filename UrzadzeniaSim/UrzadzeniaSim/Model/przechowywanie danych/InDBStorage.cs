using System.Data.Entity;
using UrzadzeniaSim.Model.RMVB.R;

namespace UrzadzeniaSim.Model.DB
{
    public class InDBStorage : TreeRepository
    {
        public static Kontekst s_Ctx;

        //nie abstract, bo ma ciało
        //virtual jest konieczne do tego, aby klasa Repo mogła ją napisać
        //"znacza to, że dana metoda może zostać nadpisana w klasie która dziedziczy po klasie w której jest ta metoda zdefiniowana"
        public virtual void saveDevice(Urzadzenie_Model device)
        {
            s_Ctx.SaveChanges();
        }

        public virtual void saveVersion(Wersja version)
        {
            s_Ctx.SaveChanges();
        }

        public void saveMeasurement(Pomiar measure)
        {
            s_Ctx.Pomiary.Add(measure);
            s_Ctx.SaveChanges();
        }

        public void saveTimeAggregate(TimeAggregate timeAggregate)
        {
            //brzydkie rozwiazanie dzieki niemu nie ma bledu
            foreach (var entry in s_Ctx.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }

            s_Ctx.TimeAggregates.Add(timeAggregate);
            s_Ctx.SaveChanges();
        }

        public void saveSpaceAggregate(SpaceAggregate spaceAggregate)
        {
            s_Ctx.SpaceAggregates.Add(spaceAggregate);
            s_Ctx.SaveChanges();
        }


        public void saveSrednia(Srednia srednia)
        {
            s_Ctx.Srednie.Add(srednia);
            s_Ctx.SaveChanges();
        }

        public void Reset()
        {
            s_Ctx.Database.ExecuteSqlCommand("DELETE FROM Wersjas");
            s_Ctx.Database.ExecuteSqlCommand("DELETE FROM Pomiars");
            s_Ctx.Database.ExecuteSqlCommand("DELETE FROM Urzadzenie_Model");
        }
    }
}
