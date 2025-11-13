using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    public class Repo : InDBStorage
    {

        //zmienic na przechowywanie samych id urzadzen i wersji
        //list, bo często sięgamy do ostatniego (największego) elementu
        public Dictionary<int, List<Wersja>> urzadzenia_wersje = new Dictionary<int, List<Wersja>>();
        public Dictionary<int, Urzadzenie> urzadzenia = new Dictionary<int, Urzadzenie>();
        //do zwrocenia wszystkich
        public List<Wersja> wersje = new List<Wersja>();

        //override jest konieczne inaczej realizowana jest wersja z klasy bazowej
        //"Modyfikator override jest wymagany do rozszerzenia lub zmodyfikowania abstrakcyjnej lub wirtualnej implementacji dziedziczonej metody, właściwości, indeksatora lub zdarzenia."
        public override void saveDevice(Urzadzenie device) {
            urzadzenia_wersje.Add(device.UrzadzenieID, new List<Wersja>());
            urzadzenia.Add(device.UrzadzenieID, device);
            base.saveDevice(device);
        }

        public override void saveVersion(Wersja v) {
            urzadzenia_wersje[v.UrzadzenieID].Add(v);
            wersje.Add(v);
            base.saveVersion(v);
        }
    }
}
