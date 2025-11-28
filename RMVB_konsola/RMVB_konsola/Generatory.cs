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
        private bool pierwszy = true;

        //rozklad jednostajny (dyskretny?)
        public (Decimal, Decimal) generujWspolrzedneDeterministycznie()
        {
            if (!pierwszy)
            {
                //rozciaglosc poludnikowa to 5 st. 50 min.
                //w minutach: 5 * 60 + 50 = 360
                //w sekundach: 350 * 60 = 21000

                //zrobic z tego fcje prywatna
                int szerokosc_stopnie = ((int)szerokosc / 1);
                int szerokosc_minuty = (int)(szerokosc % 1 / 0.01m) * 100;
                int szerokosc_sekundy = (int)((szerokosc % 0.01m) * 10000);
                int szerokosc_w_sekundach = szerokosc_stopnie * 3600 +
                    szerokosc_minuty * 60 + szerokosc_sekundy;

                szerokosc_w_sekundach += 210;

                szerokosc_stopnie = (int)(szerokosc_w_sekundach / 3600);
                szerokosc_minuty = (int)((szerokosc_w_sekundach - szerokosc_stopnie * 3600) / 60);
                szerokosc_sekundy = (int)(szerokosc_w_sekundach - szerokosc_minuty * 60 - szerokosc_stopnie * 3600);

                szerokosc = szerokosc_stopnie + szerokosc_minuty * 0.01m + szerokosc_sekundy * 0.0001m;

                dlugosc += Math.Round((10.00m / liczba_urzadzen), 4);
                dlugosc += Math.Round((0.02m / liczba_urzadzen), 4);
            }

            return (dlugosc, szerokosc);
        }

        public Rectangle generujProstokatDeterministycznie() {
            return new Rectangle(50, 15, 52, 19);
        }

    }
}
