using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola.MVB
{
    internal class DeskryptorKorzenia
    {
        DateTime poczatek;
        DateTime koniec;

        Korzen korzen;

        public DeskryptorKorzenia(DateTime poczatek, DateTime koniec, Korzen korzen)
        {
            this.poczatek = poczatek;
            this.koniec = koniec;
            this.korzen = korzen;
        }
    }
}
