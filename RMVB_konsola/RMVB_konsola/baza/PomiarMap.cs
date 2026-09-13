using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.baza
{
    internal sealed class PomiarMap:ClassMap<Pomiar>
    {
        PomiarMap() {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.dtpomiaru).Ignore();
        }
    }
}
