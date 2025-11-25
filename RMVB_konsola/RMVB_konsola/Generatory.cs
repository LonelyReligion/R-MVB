using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMVB_konsola.R;

namespace RMVB_konsola
{
    internal class Generatory
    {
        private static Decimal szerokosc = 49.0001m;
        private static Decimal dlugosc = 14.0701m;
        public static int liczba_urzadzen;

        //rozklad jednostajny (dyskretny?)
        public (Decimal, Decimal) generujWspolrzedneDeterministycznie()
        {
            szerokosc += Math.Round((5m / liczba_urzadzen), 4);
            szerokosc += Math.Round((0.50m / liczba_urzadzen), 4);

            dlugosc += Math.Round((10.00m / liczba_urzadzen), 4);
            dlugosc += Math.Round((0.02m / liczba_urzadzen), 4);

            return (dlugosc, szerokosc);

        }

        public Rectangle generujProstokatDeterministycznie() {
            return new Rectangle(50, 15, 51, 18);
        }

    }
}
