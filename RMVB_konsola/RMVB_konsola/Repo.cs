using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Repo : InDBStorage
    {
        public List<Urzadzenie> urzadzenia = new List<Urzadzenie>();

        //override jest konieczne inaczej realizowana jest wersja z klasy bazowej
        //"Modyfikator override jest wymagany do rozszerzenia lub zmodyfikowania abstrakcyjnej lub wirtualnej implementacji dziedziczonej metody, właściwości, indeksatora lub zdarzenia."
        public override void saveDevice(Urzadzenie device) {
            base.saveDevice(device);
            urzadzenia.Add(device);
        }
    }
}
