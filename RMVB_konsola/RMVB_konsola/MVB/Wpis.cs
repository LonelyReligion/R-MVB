using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    internal class Wpis
    {
        public int minKlucz;
        public int maxKlucz;
        public DateTime minData;
        public DateTime maxData;
        public Wezel wezel;

        public Wpis(int minKlucz, int maxKlucz, DateTime minData, DateTime maxData, Wezel wezel)
        {
            this.minKlucz = minKlucz;
            this.maxKlucz = maxKlucz;
            this.minData = minData;
            this.maxData = maxData;
            this.wezel = wezel;
        }
    }
}
