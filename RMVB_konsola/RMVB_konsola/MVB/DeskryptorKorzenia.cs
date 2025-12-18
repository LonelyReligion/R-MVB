using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RMVB_konsola.MVB
{
    //"object that contains information about the time range in which the root is valid and the root identificator."
    internal class DeskryptorKorzenia
    {
        DateTime poczatek;
        DateTime koniec;

        private Korzen korzen;

        public DeskryptorKorzenia(DateTime poczatek, DateTime koniec, Korzen korzen)
        {
            this.poczatek = poczatek;
            this.koniec = koniec;
            this.korzen = korzen;
        }

        public Korzen zwrocKorzen() { 
            return korzen;
        }

        public void ustawPoczatek(DateTime dt) { 
            poczatek = dt;
        }

        public void ustawKoniec(DateTime dt) { 
            koniec = dt;
        }
    }
}
