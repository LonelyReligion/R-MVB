using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMVB_konsola.R;

namespace RMVB_konsola
{
    internal class InDBStorage : TreeRepository
    {
        public void saveDevice(Urzadzenie device)
        {
            using (var ctx = new Kontekst())
            {
                Console.WriteLine(device.UrzadzenieID);
                ctx.Urzadzenia.AddOrUpdate(device);
                ctx.SaveChanges();
            }
        }
        public void saveMeasurement(Pomiar measure)
        {
            using (var ctx = new Kontekst())
            {
                ctx.Pomiary.Add(measure);
                ctx.SaveChanges();
            }
        }

        public void saveTimeAggregate(TimeAggregate timeAggregate)
        {

            using (var ctx = new Kontekst())
            {
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

        public void saveSrednia(Srednia srednia)
        {
            using (var ctx = new Kontekst())
            {
                ctx.Srednie.Add(srednia);
                ctx.SaveChanges();
            }
        }    

    }
}
