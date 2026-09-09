using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.baza
{
    public sealed class UrzadzenieMap : ClassMap<Urzadzenie>
    {
        public UrzadzenieMap() {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.rTimeAggregate).Ignore();
        }
    }
}
